using MySqlConnector;

namespace OpenCV.ConnectDB
{
    class CauHinh
    {
        public static MySqlConnection GetDBConnection(string host, int port, string database, string username, string password)
        {
            string connect = "Server=" + host + ";Database=" + database + ";User=" + username

                + ";Port=" + port + ";Password=" + password + ";SSL Mode = None";

            MySqlConnection conn = new MySqlConnection(connect);

            return conn;
        }
    }
}
