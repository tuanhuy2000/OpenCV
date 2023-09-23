using Emgu.CV.Face;
using Emgu.CV.Structure;
using Emgu.CV;
using MySqlConnector;
using OpenCV.Common;
using OpenCV.ConnectDB;
using OpenCV.Controller;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Emgu.CV.CvEnum;
using System.Collections;
using System.Diagnostics;

namespace OpenCV.View
{
    public partial class Form4 : Form
    {
        Com cm;
        ManageUser mu;

        List<string> list_detail;

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

        static public Dictionary<string, bool> Check = new Dictionary<string, bool>();
        public Form4()
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
                button3.Enabled = false;
            }
            if (checkper("DEL") != true)
            {
                mu.update_Permission(connect, 0, Int32.Parse(textBox1.Text));
            }
            button3.Visible = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            TrainImagesFromDir();
            button4.Enabled = false;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Form2 fmain = new Form2();
            fmain.Show();
            this.Close();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Employee em = new Employee(Int32.Parse(textBox1.Text), textBox2.Text);
            mu.update(connect, em, Int32.Parse(textBox1.Text));
            if (checkper("DEL") != true)
            {
                mu.update_Permission(connect, 0, Int32.Parse(textBox1.Text));
            }
            button5.Visible = false;
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            button4.Enabled = false;
            button3.Enabled = false;
            button2.Enabled = false;
            button3.Visible = false;
            button5.Visible = false;
            Employee ex = mu.GetEmployee(connect, Int32.Parse(Form2.ID_USER));
            textBox2.Text = ex.Name;
            textBox1.Text = ex.Id.ToString();
            list_detail = mu.list_per(connect, mu.id_per(connect, Form2.ID_USER));
            if (checkper("EDIT") == true)
            {
                button3.Visible = true;
                button5.Visible = true;
            }
        }

        private void ProcessFrame(object sender, EventArgs e)
        {
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
                            byte[] a = cm.converterDemo(resultImage.Resize(200, 200, Inter.Cubic));
                            string tmp = textBox2.Text;
                            if (!string.IsNullOrEmpty(textBox1.Text))
                            {
                                mu.SaveToDB(connect, a, tmp, Int32.Parse(Form2.ID_USER));
                                if (checkper("DEL") != true)
                                {
                                    mu.update_Permission(connect, 0, Int32.Parse(textBox1.Text));
                                }
                            }
                            else
                            {
                                MessageBox.Show("Nhập tên !!!");
                            }
                        }
                        EnableSaveImage = false;

                        if (isTrained)
                        {
                            Image<Gray, Byte> grayFaceResult = resultImage.Convert<Gray, Byte>().Resize(200, 200, Inter.Cubic);
                            CvInvoke.EqualizeHist(grayFaceResult, grayFaceResult);
                            FaceRecognizer.PredictionResult result = recognizer.Predict(grayFaceResult);
                            Debug.WriteLine(result.Label + ". " + result.Distance);

                            //Here results found known faces
                            if (result.Label != -1 && result.Distance < 6000)
                            {
                                CvInvoke.PutText(currentFrame, PersonsNames[result.Label], new Point(face.X - 2, face.Y - 2),
                                       FontFace.HersheyComplex, 1.0, new Bgr(Color.Orange).MCvScalar);
                                CvInvoke.Rectangle(currentFrame, face, new Bgr(Color.Green).MCvScalar, 2);
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
            int ImagesCount = 0;
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
