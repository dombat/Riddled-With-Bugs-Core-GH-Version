namespace RiddledWithBugsCoreLibrary.FlawedCode
{
    using Microsoft.Data.SqlClient;
    using System.Net;
    using Microsoft.Extensions.Configuration;
    using Microsoft.WindowsAzure.Storage.Auth;
    using System.Data;

    internal class VulnerableClass
    {
        #region Constructor
        private readonly IConfiguration configuration;
        private static PoorCode poorCode;
        public VulnerableClass(IConfiguration config)
        {
            configuration = config;

            poorCode = new PoorCode();
            
            HardcodedPassword_1();
            HardcodedPassword_2();
            HardcodedPasswordInConfig("1");
            ShouldDispose();
            SqlInjection(1); //"magic number" - what is 1?
            var result = SqlTwo("2");


        }
        #endregion

        public string password = "TWFuIGlzIGRpc3Rpbmd1aXNoZWQsIG5vdCBvbmx5IGJ5IGhpcyByZWFzb24sIGJ1dCBieSB0aGlzCBvbmx5IGJ==";//vuln #1 (when used with method HardcodedPassword_2 )

        private static void HardcodedPassword_1()
        {
            using (var client = new WebClient())
            {
                client.Credentials = new NetworkCredential("UserName", "TWFuIGlzIGRpc3Rpbmd1aXNoZWQsIG5vdCBvbmx5IGJ5IGhpcyByZWFzb24sIGJ1dCBieSB0aGlzCBvbmx5IGJ==", "Domain"); //vuln #2 (hardcoded password)
            }
        }

        private void HardcodedPassword_2()
        {
            using (var client = new WebClient())
            {
                client.Credentials = new NetworkCredential("UserName", password, "Domain"); //vuln #3 (using hardcoded password in variable)
            }
        } 


        private void HardcodedPasswordInConfig(string thing)
        {
            var cmd = new SqlCommand
            {
                CommandText = "SELECT * FROM TableA WHERE x = '" + thing + "'",
                CommandType = System.Data.CommandType.Text

            };

            using (var client = new SqlConnection())
            {
                client.ConnectionString = configuration.GetConnectionString("myDb1"); //vuln #4 (see config - contains hardcoded secrets)
                cmd.Connection = client;
                cmd.ExecuteScalar();
            }
        }

        internal static void ShouldDispose()
        {
            var con = new System.Timers.Timer//memory leak - not disposed
            {
                AutoReset = false
            };
            con.Stop();
        }

        internal void SqlInjection(int fromClient)
        {

            SqlDataReader reader = null;
            var commandText = $"SELECT * FROM Table WHERE SomeColumn = '{fromClient.ToString()}';";

            using (var command = new SqlCommand
            {
                CommandText = commandText, //vuln #5 (SQLi using concatenated string) AND ToString can be overridden (CA2100 SDL rules)
                CommandType = System.Data.CommandType.Text,
                Connection = new SqlConnection(),
            })
            {
                reader = command.ExecuteReader(); //memory leak #2?

                #region Hiding bit that is not interesting
                while (reader.Read())
                {
                    //do something
                }
                #endregion
            }

        }

        private static DataSet SqlTwo(string input)
        {

            using (var connection = new SqlConnection("jhghjH"))
            {
                var query1 = "SELECT ITEM,PRICE FROM PRODUCT WHERE ITEM_CATEGORY='"
                  + input + "' ORDER BY PRICE";
                var adapter = new SqlDataAdapter(query1, connection);
                var result = new DataSet();
                adapter.Fill(result);
                return result;
            }
        }

        public static void StorageCredentialsHardCoded()
        {
            var creds = new StorageCredentials("CredScan", "TWFuIGlzIGRpc3Rpbmd1aXNoZWQsIG5vdCBvbmx5IGJ5IGhpcyByZWFzb24sIGJ1dCBieSB0aGlzCBvbmx5IGJ==", "MyCreds");

        }
    }
}

