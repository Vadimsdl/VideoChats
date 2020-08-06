using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;

namespace Client
{
    public partial class VideoFilter : Form
    {
        public static VideoFilter self;
        public FilterInfoCollection filcol;
        VideoCaptureDevice videocap;

        public VideoFilter()
        {
            InitializeComponent();
          
           
            self = this;
            filcol = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            panel1.BackColor = ColorTranslator.FromHtml("#03899C");

            for (int i = 0; i < filcol.Count; i++)
            {
               comboBox1.Items.Add(filcol[i].Name.ToString());
            }
          
        }
        bool flag = false;
        int last = 0;
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox1.BackColor = ColorTranslator.FromHtml("#009B95");
            label1.Visible = false;
            Rt();
        }

        void Rt()
        {
            try
            {
                if (!flag)
                {
                    videocap = new VideoCaptureDevice(filcol[comboBox1.SelectedIndex].MonikerString);
                    videocap.NewFrame += Videoframe;
                    videocap.Start();
                    flag = true;
                }
                else
                {
                    videocap.Stop();
                    videocap.SignalToStop();
                    flag = false;
                    Rt();
                }
            }
            catch { }
        }

        private void Videoframe(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();
                pictureBox1.BackgroundImage = bitmap;
            }
            catch
            {

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Form1.self.SelectCam = comboBox1.SelectedIndex;
                this.Close();
                tr = false;
            }
            catch
            {
                MessageBox.Show("Выберете камеру !!!");
            }
        }

        private void VideoFilter_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
               
                    videocap.SignalToStop();
                    videocap.Stop();
           
            }
            catch
            {
               // MessageBox.Show("Вы не выбрали камеру!!!");
            }
        }

        private void VideoFilter_Load(object sender, EventArgs e)
        {
            this.BackColor = ColorTranslator.FromHtml("#5CCDC9");
            panel1.BackColor = ColorTranslator.FromHtml("#1D7471");
            pictureBox1.BackColor = ColorTranslator.FromHtml("#009B95");
            button1.BackColor = ColorTranslator.FromHtml("#006561");
            comboBox1.BackColor = ColorTranslator.FromHtml("#1D7471");
            comboBox1.ForeColor = Color.White;
            label4.ForeColor = ColorTranslator.FromHtml("#5CCDC9");
            label1.BackColor = ColorTranslator.FromHtml("#009B95");
            label1.ForeColor = Color.White;
        }

        private void button1_MouseMove(object sender, MouseEventArgs e)
        {
            button1.BackColor = ColorTranslator.FromHtml("#009B95");
        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
            button1.BackColor = ColorTranslator.FromHtml("#006561");
        }

        private void label4_MouseMove(object sender, MouseEventArgs e)
        {
            label4.ForeColor = ColorTranslator.FromHtml("#FFB173");
        }

        private void label4_MouseLeave(object sender, EventArgs e)
        {
            label4.ForeColor = ColorTranslator.FromHtml("#5CCDC9");

        }

        private void label4_MouseDown(object sender, MouseEventArgs e)
        {
            label4.ForeColor = ColorTranslator.FromHtml("#FF7100");
        }
        bool tr = true;
        private void label4_Click(object sender, EventArgs e)
        {
            this.Close();
            tr = true;
        }

        Point p;
        private void toolStrip1_MouseDown(object sender, MouseEventArgs e)
        {
            p = new Point(e.X, e.Y);

        }

        private void toolStrip1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Left += e.X - p.X;
                Top += e.Y - p.Y;
            }
        }
    }
}
