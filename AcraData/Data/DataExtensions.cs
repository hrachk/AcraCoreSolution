using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

namespace AcraData.Data
{
    public static class DataExtensions
    {
        
        public static List<T> RawSqlQuery<T>(this DbContext context, string query, Func<DbDataReader, T> map, Dictionary<string, object> parameters = null)
        {
            using (var command = ConstructCommand(context, query, parameters))
            {
                command.CommandText = query;
                command.CommandType = CommandType.Text;

                context.Database.OpenConnection();
                if (parameters != null)
                {
                    foreach (var p in parameters)
                    {
                        var param = command.CreateParameter();
                        param.ParameterName = p.Key;
                        param.Value = p.Value;
                        command.Parameters.Add(param);
                    }
                }
                using (var result = command.ExecuteReader())
                {
                    var entities = new List<T>();

                    if (result.HasRows)
                    {
                        while (result.Read())
                        {
                            entities.Add(map(result));
                        }
                    }
                    context.Database.CloseConnection();
                    return entities;
                }
            }
        }

        #region async
       

        public async static Task<List<T>> RawSqlQueryAsync<T>(this DbContext context, string query, Func<DbDataReader, T> map, Dictionary<string, object> parameters = null)
        {
            using (var command = ConstructCommand(context, query, parameters))
            {
                using (var result = await command.ExecuteReaderAsync())
                {
                    var entities = new List<T>();

                    if (result.HasRows)
                    {
                        while (await result.ReadAsync())
                        {
                            entities.Add(map(result));
                        }
                    }
                    return entities;
                }
            }
        }

        public async static Task<DataTable> GetDataTableSchemaAsync(this DbContext context, string query, Dictionary<string, object> parameters = null)
        {
            using (var command = ConstructCommand(context, query, parameters))
            {
                using (var result = await command.ExecuteReaderAsync())
                {
                    if (result.HasRows)
                    {
                        return await Task.Run(() => result.GetSchemaTable());
                    }
                    return null;
                }
            }
        }
       
        #endregion

        private static DbCommand ConstructCommand(DbContext context, string query, Dictionary<string, object> parameters = null)
        {
            var command = context.Database.GetDbConnection().CreateCommand();
            command.CommandText = query;
            command.CommandType = CommandType.Text;

            context.Database.OpenConnection();
            if (parameters != null)
            {
                foreach (var p in parameters)
                {
                    var param = command.CreateParameter();
                    param.ParameterName = p.Key;
                    param.Value = p.Value;
                    command.Parameters.Add(param);
                }
            }
            context.Database.CloseConnection();
            return command;
        }


        public static void AddOrUpdate(this DbContext ctx, object entity)
        {
            var entry = ctx.Entry(entity);
            switch (entry.State)
            {
                case EntityState.Detached:
                    ctx.Add(entity);
                    break;
                case EntityState.Modified:
                    ctx.Update(entity);
                    break;
                case EntityState.Added:
                    ctx.Add(entity);
                    break;
                case EntityState.Unchanged:
                    //item already in db no need to do anything  
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
