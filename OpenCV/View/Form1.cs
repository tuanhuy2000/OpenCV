using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Face;
using Emgu.CV.Structure;
using MySqlConnector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using OpenCV.Common;
using OpenCV.Controller;
using OpenCV.ConnectDB;
using OpenCV.View;

namespace OpenCV
{
    public partial class Form1 : Form
    {
        Com cm;
        ManageUser mu;

        MySqlConnection connect = KetNoi.GetDBConnection();

        private VideoCapture videoCapture = null;
        private Image<Bgr, Byte> currentFrame = null;
        Mat frame = new Mat();
        private bool FacesDetectionEnabled = false;
        CascadeClassifier faceCascadeClassifier = new CascadeClassifier("..\\..\\haarcascade_frontalface_alt.xml");
        bool EnableSaveImage = false;

        List<Image<Gray, Byte>> TrainedFaces = new List<Image<Gray, byte>>();
        List<int> PersonsLabes = new List<int>();
        EigenFaceRecognizer recognizer;
        private bool isTrained = false;
        List<string> PersonsNames = new List<string>();

        Dictionary<string, bool> Check = new Dictionary<string, bool>();

        public Form1()
        {
            cm = new Com();
            mu = new ManageUser();
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            videoCapture = new VideoCapture();
            videoCapture.ImageGrabbed += ProcessFrame;
            videoCapture.Start();
            button1.Enabled = false;
            button2.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FacesDetectionEnabled = true;
            button2.Enabled = false;
            button3.Enabled = true;
            button4.Enabled = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            EnableSaveImage = true;
            if (!string.IsNullOrEmpty(textBox1.Text))
            {
                button4.Enabled = true;
                button5.Enabled = false;
                button3.Enabled = false;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            TrainImagesFromDir();
            //Check = Form4.Check;
            button4.Enabled = false;
            button5.Enabled = true;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //MySqlConnection connect = KetNoi.GetDBConnection();
            ArrayList list = mu.ReadInfor(connect);
            int count = list.Count;
            for (int i = 0; i < count; i++)
            {
                foreach (Employee item in list)
                {
                    if (!Check[item.Name])
                    {
                        list.Remove(item);
                        break;
                    }
                }
            }
            dataGridView1.DataSource = list;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Form2 fmain = new Form2();
            fmain.Show();
            this.Close();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            ArrayList list = mu.ReadInfor(connect);
            dataGridView1.DataSource = list;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Employee em = new Employee(Int32.Parse(textBox2.Text), textBox1.Text);
            mu.update(connect, em, Int32.Parse(textBox2.Text));
            textBox1.Clear();
            textBox2.Clear();
            button8.Enabled = false;
            button9.Enabled = false;
        }

        private void button9_Click(object stender, EventArgs e)
        {
            mu.update_Permission(connect, 1, Int32.Parse(textBox2.Text));
            textBox1.Clear();
            textBox2.Clear();
            button9.Enabled = false;
            button8.Enabled = false;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = new DataGridViewRow();
            row = dataGridView1.Rows[e.RowIndex];
            textBox2.Text = Convert.ToString(row.Cells["Id"].Value);
            textBox1.Text = Convert.ToString(row.Cells["Name"].Value);
            button8.Enabled = true;
            button9.Enabled = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //MySqlConnection connect = KetNoi.GetDBConnection();
            button9.Enabled = false;
            button5.Enabled = false;
            button4.Enabled = false;
            button3.Enabled = false;
            button2.Enabled = false;
            button8.Enabled = false;
        }

        private void ProcessFrame(object sender, EventArgs e)
        {
            //MySqlConnection connect = KetNoi.GetDBConnection();
            videoCapture.Retrieve(frame, 0);
            currentFrame = frame.ToImage<Bgr, Byte>().Resize(pictureBox1.Width, pictureBox1.Height, Inter.Cubic);

            if (FacesDetectionEnabled)
            {
                Mat grayImage = new Mat();
                CvInvoke.CvtColor(currentFrame, grayImage, ColorConversion.Bgr2Gray);
                CvInvoke.EqualizeHist(grayImage, grayImage);
                Rectangle[] faces = faceCascadeClassifier.DetectMultiScale(grayImage, 1.1, 3, Size.Empty, Size.Empty);
                if (faces.Length > 0)
                {
                    foreach (var face in faces)
                    {
                        CvInvoke.Rectangle(currentFrame, face, new Bgr(Color.Red).MCvScalar, 2);
                        Image<Bgr, Byte> resultImage = currentFrame.Convert<Bgr, Byte>();
                        resultImage.ROI = face;
                        pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
                        pictureBox2.Image = resultImage.AsBitmap();

                        if (EnableSaveImage)
                        {
                            //Task.Factory.StartNew(() =>
                            //{
                            //for (int i = 0; i < 1; i++)
                            //{
                            byte[] a = cm.converterDemo(resultImage.Resize(200, 200, Inter.Cubic));
                            string tmp = textBox1.Text;
                            if (!string.IsNullOrEmpty(textBox1.Text))
                            {
                                mu.SaveToDB(connect, a, tmp,Int32.Parse(Form2.ID_USER));
                            }
                            else
                            {
                                MessageBox.Show("Nhập tên !!!");
                            }
                            //Thread.Sleep(1000);
                            //}
                            //});
                        }
                        EnableSaveImage = false;

                        if (isTrained)
                        {
                            Image<Gray, Byte> grayFaceResult = resultImage.Convert<Gray, Byte>().Resize(200, 200, Inter.Cubic);
                            CvInvoke.EqualizeHist(grayFaceResult, grayFaceResult);
                            FaceRecognizer.PredictionResult result = recognizer.Predict(grayFaceResult);

                            //pictureBox1.Image = grayFaceResult.AsBitmap();
                            //pictureBox2.Image = TrainedFaces[result.Label].AsBitmap();

                            Debug.WriteLine(result.Label + ". " + result.Distance);

                            //Here results found known faces
                            if (result.Label != -1 && result.Distance < 6000)
                            {
                                CvInvoke.PutText(currentFrame, PersonsNames[result.Label], new Point(face.X - 2, face.Y - 2),
                                       FontFace.HersheyComplex, 1.0, new Bgr(Color.Orange).MCvScalar);
                                CvInvoke.Rectangle(currentFrame, face, new Bgr(Color.Green).MCvScalar, 2);
                                string b = PersonsNames[1];
                                bool a = Check[PersonsNames[result.Label]];
                                if (!Check[PersonsNames[result.Label]])
                                {
                                    Check[PersonsNames[result.Label]] = true;
                                }
                            }
                            //here results did not found any know faces
                            else
                            {
                                CvInvoke.PutText(currentFrame, "Unknown", new Point(face.X - 2, face.Y - 2),
                                    FontFace.HersheyComplex, 1.0, new Bgr(Color.Orange).MCvScalar);
                                CvInvoke.Rectangle(currentFrame, face, new Bgr(Color.Red).MCvScalar, 2);
                            }
                        }

                    }
                }
            }
            pictureBox1.Image = currentFrame.AsBitmap();
        }

        private bool TrainImagesFromDir()
        {
            //MySqlConnection connect = KetNoi.GetDBConnection();
            int ImagesCount = 0;
            //double Threshold = 2000;
            TrainedFaces.Clear();
            PersonsLabes.Clear();
            PersonsNames.Clear();
            Check.Clear();
            try
            {
                ArrayList list = mu.ReadFromDB(connect);
                ArrayList listName = mu.ReadName(connect);

                foreach (byte[] a in list)
                {
                    Image<Bgr, byte> trained = cm.ByteToImage(a);
                    Image<Gray, byte> trainedImage = trained.Convert<Gray, Byte>().Resize(200, 200, Inter.Cubic);
                    CvInvoke.EqualizeHist(trainedImage, trainedImage);
                    TrainedFaces.Add(trainedImage);
                    PersonsLabes.Add(ImagesCount);
                    ImagesCount++;
                }
                foreach (string a in listName)
                {
                    PersonsNames.Add(a);
                    Check.Add(a, false);
                }

                if (TrainedFaces.Count() > 0)
                {
                    List<Mat> a = new List<Mat>();
                    foreach (Image<Gray, byte> item in TrainedFaces)
                    {
                        Mat x = item.Mat;
                        a.Add(x);
                    }
                    recognizer = new EigenFaceRecognizer(ImagesCount);
                    recognizer.Train(a.ToArray(), PersonsLabes.ToArray());
                    isTrained = true;
                    return true;
                }
                else
                {
                    isTrained = false;
                    return false;
                }
            }
            catch (Exception ex)
            {
                isTrained = false;
                MessageBox.Show("Error in Train Images: " + ex.Message);
                return false;
            }

        }
    }
}
