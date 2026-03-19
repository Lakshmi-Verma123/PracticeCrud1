using Microsoft.Data.SqlClient;
namespace PracticeCrud1.Common
{
    public class MyConnection
    {
        public static string DefaultConnection { get; set; }
    }
    public class ORMConnection
    {
        private static SqlConnection con;
        public static SqlConnection GetSqlConnection()
        {
            con = new SqlConnection(MyConnection.DefaultConnection);
            return con;
        }
    }
}
