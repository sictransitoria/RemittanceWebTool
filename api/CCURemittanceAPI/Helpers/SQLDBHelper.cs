using System.Data;
using System.Data.SqlClient;

namespace CCURemittanceAPI.Helpers
{
    public class SQLDBHelper
    {
        private static string arrowDBServer = "";
        private static string arrowDBName = "";
        private static string arrowDBUser = "";
        private static string arrowDBPass = "";
        private static string arrowConnStr = "Server=" + arrowDBServer + ";"
                           + "Database=" + arrowDBName + ";"
                           + "uid=" + arrowDBUser + ";"
                           + "password=" + arrowDBPass;

        private static string formviewerDBServer = "";
        private static string formviewerDBName = "";
        private static string formviewerDBUser = "";
        private static string formviewerDBPass = "";
        private static string formviewerConnStr = "Server=" + formviewerDBServer + ";"
                           + "Database=" + formviewerDBName + ";"
                           + "uid=" + formviewerDBUser + ";"
                           + "password=" + formviewerDBPass;

        public static string getConnectionString(string server, string db)
        {

            string ConnStr = "Server=" + server + ";"
                          + "Database=" + db + ";"
                          + "uid=;"
                          + "password=";
            return ConnStr;
        }

        public static DataTable QueryDbDataTableArrow(string queryString)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(arrowConnStr))
            {
                //Create command 
                SqlCommand command = new SqlCommand(queryString, conn);

                SqlDataAdapter dataAdapter = new SqlDataAdapter(command);

                command.Connection.Open();

                dataAdapter.Fill(dt);

            }
            return dt;
        }

        public static DataTable QueryDbDataTableFormviewer(string queryString)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(formviewerConnStr))
            {
                //Create command 
                SqlCommand command = new SqlCommand(queryString, conn);

                SqlDataAdapter dataAdapter = new SqlDataAdapter(command);

                command.Connection.Open();
                //command.CommandTimeout = 0;

                dataAdapter.Fill(dt);

            }
            return dt;
        }

        public static DataTable QueryDbDataTable(string server, string db, string queryString, params SqlParameter[] parameters)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(getConnectionString(server, db)))
            {
                //Create command 
                SqlCommand command = new SqlCommand(queryString, conn);

                command.Parameters.AddRange(parameters);

                SqlDataAdapter dataAdapter = new SqlDataAdapter(command);

                command.Connection.Open();
                //command.CommandTimeout = 30;

                dataAdapter.Fill(dt);

            }
            return dt;
        }
    }
}