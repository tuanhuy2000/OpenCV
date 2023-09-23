using MySqlConnector;

namespace OpenCV.ConnectDB
{
    class KetNoi
    {
        public static MySqlConnection GetDBConnection()
        {
            string host = "localhost";
            int port = 3306;
            string database = "test";
            string username = "root";
            string password = "";
            return CauHinh.GetDBConnection(host, port, database, username, password);
        }
    }
}
