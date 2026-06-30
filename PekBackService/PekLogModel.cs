using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using AcraData.Data;
using Newtonsoft.Json;

namespace PekBackService
{
    public class PekJournalModel
    {
        private readonly DbContextOptions<Acra3DbContext> _acra3DbOptions;
        private readonly DbContextOptions<AcraJournalDbContext> _acraJournalOptions;
        private readonly AcraUtils.Configuration.ValidatorConfig _configuration;
        private readonly ElasticJournalService _elastic;
        private readonly string _logPath;

        public PekJournalModel(
            DbContextOptions<Acra3DbContext> acra3dbOptions,
            DbContextOptions<AcraJournalDbContext> acraJournalOptions,
            IOptions<AcraUtils.Configuration.ValidatorConfig> configuration,
            ElasticJournalService elastic)
        {
            _acra3DbOptions = acra3dbOptions;
            _acraJournalOptions = acraJournalOptions;
            _configuration = configuration.Value;
            _elastic = elastic;
            _logPath = Path.Combine(Directory.GetCurrentDirectory(), "log.log");
        }

        public async Task<string> LogPekResponsesModel(
            PEK_ServiceReference.Response response,
            string request,
            bool isTin,
            long userActivityId,
            int source)
        {
            string errors = string.Empty;

            // 1. Логирование в AcraJournalDb + Elasticsearch
            await LogJournalAsync(request, response, userActivityId, source);

            // 2. Валидация данных через Acra3Db
            errors = ValidateResponse(response);

            return errors;
        }

        private async Task LogJournalAsync(string request, PEK_ServiceReference.Response response, long userActivityId, int source)
        {
            try
            {
                using var context = new AcraJournalDbContext(_acraJournalOptions);

                var journalEntity = new AcraData.Models.AcraJournal.Pek_Journal
                {
                    Request = request,
                    Response = JsonConvert.SerializeObject(response),
                    ResponseDateTime = DateTime.Now,
                    ErrorText = response.errorMessage,
                    UserActivityId = userActivityId,
                    Status = Int32.Parse(response.errorCode),
                    SourceID = source
                };

                context.Pek_Journal.Add(journalEntity);
                await context.SaveChangesAsync();

                // Elastic
                var doc = new PekJournalDocument
                {
                    Request = request,
                    Response = response,
                    UserActivityId = userActivityId,
                    Status = response.errorCode,
                    SourceId = source,
                    CreatedAt = DateTime.UtcNow
                };

                await _elastic.LogAsync(doc);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now} Unable to log PEK journal");
                File.AppendAllText(_logPath, $"\n\r{DateTime.Now} {ex}\n\r");
            }
        }

        private string ValidateResponse(PEK_ServiceReference.Response response)
        {
            string errors = string.Empty;

            try
            {
                using var context = new Acra3DbContext(_acra3DbOptions);

                var taxDebtType = context.Pek_Definitions
                    .Where(x => x.parameter == "TaxDebtType")
                    .Select(x => x.acceptablevalue)
                    .ToList();

                var organizationType = context.Pek_Definitions
                    .Where(x => x.parameter == "OrganizationType")
                    .Select(x => x.acceptablevalue)
                    .ToList();

                var errorCode = context.Pek_Definitions
                    .Where(x => x.parameter == "errorCode")
                    .Select(x => x.acceptablevalue)
                    .ToList();

                foreach (var item in response.TaxDebts)
                {
                    if (!taxDebtType.Contains(item.TaxDebtType))
                    {
                        errors = "taxDebtType";
                        break;
                    }
                }

                var orgType = response.OrganizationType.Contains(' ')
                    ? response.OrganizationType.Substring(0, response.OrganizationType.IndexOf(' '))
                    : response.OrganizationType;

                if (!organizationType.Contains(orgType))
                    errors = string.IsNullOrEmpty(errors)
                        ? "OrganizationType"
                        : $"{errors}&OrganizationType";

                if (!errorCode.Contains(response.errorCode))
                    errors = string.IsNullOrEmpty(errors)
                        ? "errorCode"
                        : $"{errors}&errorCode";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now} Validation failed");
                File.AppendAllText(_logPath, $"\n\r{DateTime.Now} {ex}\n\r");
            }

            return errors;
        }

        public async Task LogPekActivityAsync(long userActivityId, string message)
        {
            try
            {
                using var context = new Acra3DbContext(_acra3DbOptions);
                using var tx = await context.Database.BeginTransactionAsync();

                try
                {
                    context.Pek_ActivityLogs.Add(new AcraData.Models.Acra3.Pek_ActivityLog
                    {
                        userActivityId = userActivityId,
                        message = message,
                        date = DateTime.Now
                    });

                    await context.SaveChangesAsync();
                    await tx.CommitAsync();
                }
                catch
                {
                    await tx.RollbackAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(_logPath,
                    $"\n\r{DateTime.Now} LogPekActivity Failed: {ex}\n\r");
            }
        }
    }
}