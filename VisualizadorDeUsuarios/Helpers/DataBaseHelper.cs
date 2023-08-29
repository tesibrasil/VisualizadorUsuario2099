using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using TesiNexus.Nexus.Models;
using Dapper;
using System.Data;
using System.IO;

namespace TesiNexus.Helpers
{
    public class DataBaseHelper
    {
        public static IDbTransaction CurrentTransaction { get; private set; }

        public static bool CheckingConnection(string connStrFonte)
        {

            using (SqlConnection connection = new SqlConnection(connStrFonte))
            {
                try
                {
                    connection.Open();
                    connection.Close();
                }
                catch (SqlException)
                {
                    return false;
                }

                return true;
            }

        }

        public static string CheckingVersion(string connStrFonte)
        {
            string version = "";
            using (SqlConnection connection = new SqlConnection(connStrFonte))
            {
                connection.Open();
                try
                {
                    version = connection.Query<string>($@"SELECT CAST(Version_1 as varchar) +'.'+ CAST(Version_2 as varchar)  +'.'+ CAST(Version_3 as varchar) as 'Version'
                                                            FROM VERSION"
                     , transaction: CurrentTransaction
                     , commandType: CommandType.Text).FirstOrDefault(); ;
                }
                catch
                {

                }
                return version;
            }
        }

        public static void UpdatingVersion(string connStrFonte, string sqlFile)
        {
            string script = File.ReadAllText(sqlFile);

            using (SqlConnection connection = new SqlConnection(connStrFonte))
            {
                connection.Open();
                try
                {
                    connection.Query<string>(script
                                             ,transaction: CurrentTransaction
                                             ,commandType: CommandType.Text); 
                }
                catch
                {

                }
               
            }
        }

    }
}
