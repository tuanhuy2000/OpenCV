using System.Collections.Generic;
using System.Windows.Forms;
using MySqlConnector;
using OpenCV.ConnectDB;
using OpenCV.Controller;

namespace OpenCV.View
{
    public partial class Form2 : Form
    {
        List<string> list_detail;
        ManageUser mu;
        MySqlConnection connect = KetNoi.GetDBConnection();

        public static string ID_USER = "";

        public Form2()
        {
            mu=new ManageUser();
            InitializeComponent();
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            MySqlConnection connect = KetNoi.GetDBConnection();
            ID_USER = mu.getID(connect, textBox1.Text, textBox2.Text);
            list_detail = mu.list_per(connect, mu.id_per(connect, ID_USER));
            if (ID_USER != "" && checkper("ADMIN") == true)
            {
                Form3 form = new Form3();
                form.Show();
                this.Hide();
            }
            else if (ID_USER != "" && checkper("DEL") == true && checkper("ADMIN") == false)
            {
                Form1 fmain = new Form1();
                fmain.Show();
                this.Hide();
            }
            else if (ID_USER != "")
            {
                Form4 fmain = new Form4();
                fmain.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Tài khoảng và mật khẩu không đúng !");
            }
        }

        private bool checkper(string code)
        {
            bool check = false;
            foreach (string item in list_detail)
            {
                if (item == code)
                {
                    check = true;
                }
            }
            return check;
        }
    }
}
