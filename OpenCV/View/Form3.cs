using MySqlConnector;
using OpenCV.ConnectDB;
using OpenCV.Controller;
using System;
using System.Collections;
using System.Windows.Forms;

namespace OpenCV.View
{
    public partial class Form3 : Form
    {
        ManageUser mu;
        MySqlConnection connect = KetNoi.GetDBConnection();
        public Form3()
        {
            mu = new ManageUser();
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            ArrayList array = mu.GetMajor(connect);
            comboBox1.DataSource = array;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox1.Text))
            {
                mu.Add_Major(connect, textBox1.Text);
                textBox1.Clear();
            }
            else
            {
                MessageBox.Show("Nhập ngành !!!");
            }
        }
    }
}
