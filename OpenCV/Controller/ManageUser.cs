using MySqlConnector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Windows.Forms;

namespace OpenCV.Controller
{
    class ManageUser
    {
        /// <summary>
        /// gv and hs
        /// </summary>
        
        public void SaveToDB(MySqlConnection connect, byte[] a, string na, int id)
        {
            connect.Open();
            string sql = "UPDATE images SET imgg = @img, name = @na WHERE id = @id";
            MySqlCommand command = new MySqlCommand();
            command.Connection = connect;
            command.CommandText = sql;
            command.Parameters.AddWithValue("@img", a);
            command.Parameters.AddWithValue("@na", na);
            command.Parameters.AddWithValue("@id", id);
            int rows = command.ExecuteNonQuery();
            connect.Close();
        }

        public ArrayList ReadFromDB(MySqlConnection connect)
        {
            ArrayList list = new ArrayList();
            connect.Open();
            string sql1 = "SELECT imgg FROM images";
            MySqlCommand command1 = new MySqlCommand();
            command1.Connection = connect;
            command1.CommandText = sql1;
            using (DbDataReader reader = command1.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        byte[] imageBytes = (byte[])reader["imgg"];
                        list.Add(imageBytes);
                    }
                }
            }
            connect.Close();
            return list;
        }

        public ArrayList ReadName(MySqlConnection connect)
        {
            ArrayList list = new ArrayList();
            connect.Open();
            string sql1 = "SELECT name FROM images";
            MySqlCommand command1 = new MySqlCommand();
            command1.Connection = connect;
            command1.CommandText = sql1;
            using (DbDataReader reader = command1.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        string tmp = reader.GetString(0);
                        list.Add(tmp);
                    }
                }
            }
            connect.Close();
            return list;
        }

        public ArrayList ReadInfor(MySqlConnection connect)
        {
            ArrayList list = new ArrayList();
            connect.Open();
            string sql1 = "SELECT id,name FROM images";
            MySqlCommand command1 = new MySqlCommand();
            command1.Connection = connect;
            command1.CommandText = sql1;
            using (DbDataReader reader = command1.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        string tmp = reader.GetString(1);
                        int id = reader.GetInt32(0);
                        Employee employee = new Employee(id, tmp);
                        list.Add(employee);
                    }
                }
            }
            connect.Close();
            return list;
        }

        public string getID(MySqlConnection connect, string username, string pass)
        {
            string id = "";
            try
            {
                connect.Open();
                string sql1 = "SELECT * FROM images WHERE user_name = @user_name and pass = @pass";
                MySqlCommand command = new MySqlCommand();
                command.Connection = connect;
                command.Parameters.AddWithValue("@user_name", username);
                command.Parameters.AddWithValue("@pass", pass);
                command.CommandText = sql1;
                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            id = reader["id"].ToString();
                        }
                    }
                }
                connect.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("Lỗi xảy ra khi truy vấn dữ liệu hoặc kết nối với server thất bại !");
            }
            return id;
        }

        public string id_per(MySqlConnection connect, string id_user)
        {
            string id = "";
            try
            {
                connect.Open();
                MySqlCommand command = new MySqlCommand("SELECT * FROM relationship WHERE id_user_rel ='" + id_user + "'", connect);
                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            id = reader["id_per_rel"].ToString();
                        }
                    }
                }
                connect.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("Lỗi xảy ra khi truy vấn dữ liệu hoặc kết nối với server thất bại !");
            }
            return id;
        }

        public List<string> list_per(MySqlConnection connect, string id_per)
        {
            List<string> termsList = new List<string>();
            try
            {
                connect.Open();
                MySqlCommand command = new MySqlCommand("SELECT * FROM permission_detail WHERE id_per <='" + id_per + "'", connect);
                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            termsList.Add(reader["code_action"].ToString());
                        }
                    }
                }
                connect.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return termsList;
        }

        public int update(MySqlConnection connect, Employee em, int id)
        {
            connect.Open();
            string sql = "UPDATE images SET name = @name WHERE id = @id";
            MySqlCommand command = new MySqlCommand();
            command.Connection = connect;
            command.CommandText = sql;
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@name", em.Name);
            int rows = command.ExecuteNonQuery();
            connect.Close();
            return rows;
        }

        public int update_Permission(MySqlConnection connect, int per, int id)
        {
            connect.Open();
            string sql = "UPDATE relationship SET id_per_rel = @per WHERE id_user_rel = @id";
            MySqlCommand command = new MySqlCommand();
            command.Connection = connect;
            command.CommandText = sql;
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@per", per);
            int rows = command.ExecuteNonQuery();
            connect.Close();
            return rows;
        }

        public Employee GetEmployee(MySqlConnection connect, int id)
        {
            Employee ex = null;
            connect.Open();
            MySqlCommand command = new MySqlCommand("SELECT * FROM images WHERE id ='" + id + "'", connect);
            using (DbDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ex = new Employee(Int32.Parse(reader["id"].ToString()), reader["name"].ToString());
                    }
                }
            }
            connect.Close();
            return ex;
        }

        /// <summary>
        /// admin
        /// </summary>
        
        public ArrayList GetMajor(MySqlConnection connect)
        {
            ArrayList list = new ArrayList();
            connect.Open();
            MySqlCommand command = new MySqlCommand("SELECT name_mj FROM majors", connect);
            using (DbDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        String tmp = reader["name_mj"].ToString();
                        list.Add(tmp);
                    }
                }
            }
            connect.Close();
            return list;
        }

        public void Add_Major(MySqlConnection connect,string ma)
        {
            connect.Open();
            MySqlCommand command = new MySqlCommand("INSERT INTO `majors`(`name_mj`) VALUES ('"+ma+"')", connect);
            int rows = command.ExecuteNonQuery();
            connect.Close();
        }
    }
}
