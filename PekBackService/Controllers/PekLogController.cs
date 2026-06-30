using AcraData.Data;
using Microsoft.AspNetCore.Mvc;
using PekBackService;
using System.Threading.Tasks;

namespace PekBackService.Controllers
{
    [ApiController]
    [Route("PekJournal")]
    public class PekJournalController : ControllerBase
    {
        private readonly PekJournalModel _journal;

        public PekJournalController(PekJournalModel journal)
        {
            _journal = journal;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return Ok(new { status = "PekJournal API is running" });
        }

        [HttpPost("LogPekResponses")]
        public async Task<IActionResult> LogPekResponses([FromBody] AcraUtils.PekReqModel pekReq)
        {
            if (pekReq == null)
                return BadRequest(new { status = "error", message = "Request body is null" });

            var errors = await _journal.LogPekResponsesModel(
                pekReq.responseModel,
                pekReq.requestModel,
                pekReq.isTinModel,
                pekReq.userActivityId,
                pekReq.SourceID
            );

            return Ok(new { status = "success", validationErrors = errors });
        }

        [HttpPost("LogPekActivity")]
        public async Task<IActionResult> LogPekActivity(long userActivityId, string message)
        {
            if (string.IsNullOrEmpty(message))
                return BadRequest(new { status = "error", message = "Message is empty" });

            await _journal.LogPekActivityAsync(userActivityId, message);
            return Ok(new { status = "success" });
        }
    }
}