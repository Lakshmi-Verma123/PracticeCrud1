using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using PracticeCrud1.Common;

namespace PracticeCrud1.Common.DAL
{
    public class DapperContext : IDapperContext
    {
        // Query single record
        public async Task<T> QueryFirst<T>(string procedureName, object parameters)
        {
            dynamic result = null;
            using (IDbConnection db = ORMConnection.GetSqlConnection())
            {
                try
                {
                    db.Open();
                    result = await db.QueryFirstOrDefaultAsync<T>(procedureName, parameters, commandType: CommandType.StoredProcedure);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message.ToString());
                }
                finally
                {
                    db.Close();
                }
            }
            return result;
        }
        public async Task<IEnumerable<T>> QueryAll<T>(string procedureName, object parameters)
        {
            IEnumerable<T> list = new List<T>();
            using (IDbConnection db = ORMConnection.GetSqlConnection())
            {
                try
                {
                    db.Open();
                    list = await db.QueryAsync<T>(procedureName, parameters, commandType: CommandType.StoredProcedure);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message.ToString());
                }
                finally
                {
                    db.Close();
                }
            }
            return list;
        }

        public Task<T> QueryFirstOrDefault<T>(string v, object param)
        {
            throw new NotImplementedException();
        }
    }
}

