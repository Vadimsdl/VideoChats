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
using NAudio.Wave;
using NAudio.FileFormats;
using NAudio.CoreAudioApi;
using NAudio;
using System.Media;
using System.Net;
using System.IO;
using System.Threading;
using AForge.Controls;


namespace Client
{
    

    public partial class Form1 : Form
    {
        public static Form1 self;
       public FilterInfoCollection filcol;
        VideoCaptureDevice videocap;
       public  FilterInfoCollection filcol1;
        public int SelectCam = 0;
        public int SelectMic = 0;
        WaveIn recorder;
     
        public BufferedWaveProvider bufferedWaveProvider;
        
        public WaveOut player;

        ConnectServer cs;
     

        public Form1()
        {
            InitializeComponent();
            self = this;
            cs = new ConnectServer();
            Webbutton.Enabled = false;
            Webbutton.BackColor = Color.Gray;
            Voicbutton.Enabled = false;
            Voicbutton.BackColor = Color.Gray;
           
           // textBox3.Text = "127.0.0.2";
            String host = System.Net.Dns.GetHostName();
            // Получение ip-адреса.
            System.Net.IPAddress ip = System.Net.Dns.GetHostByName(host).AddressList[0];
            // Показ адреса в label'е.
            textBox2.Text = ip.ToString();
          

        }

        bool flags = true;
        private void Connectbutton_Click(object sender, EventArgs e)
        {
            try
            {
                if (flags)
                {
                    int port = 32188;
                    int portAuf = 32189;
                    Webbutton.Enabled = true;
                    Voicbutton.Enabled = true;
                    Voicbutton.BackColor = ColorTranslator.FromHtml("#009B95");
                    Webbutton.BackColor = ColorTranslator.FromHtml("#009B95");
                    label1.Visible = false;
                    Connectbutton.BackgroundImage = Image.FromFile("соединение-включено.png");
                    cs.myEpVideo = new IPEndPoint(IPAddress.Parse(textBox2.Text), port);
                    ConnectServer.myPort = port;
                    cs.myEpAudio = new IPEndPoint(IPAddress.Parse(textBox2.Text), portAuf);

                    cs.startExchangeAud();
                    cs.startExchangeVid();
                    flags = false;
                }
                else
                {
                    cs.Stop();
                    Connectbutton.BackgroundImage = Image.FromFile("соединение-включен.png");
                    flags = true;
                }
            }
            catch { }
        }
        Size s; 
        private void Videoframe(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();
                pictureBox1.BackgroundImage = bitmap;
                s = eventArgs.Frame.Size;
                Bitmap bit = (Bitmap)pictureBox1.BackgroundImage.Clone();
                MemoryStream ms1 = new MemoryStream();
                bit.Save(ms1, System.Drawing.Imaging.ImageFormat.Jpeg);
                cs.Byt = ms1.ToArray();

                cs.OtpravkaVid();
            }
            catch(Exception ex ) { }
        
      
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            filcol = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            filcol1 = new FilterInfoCollection(FilterCategory.AudioInputDevice);
            this.BackColor = ColorTranslator.FromHtml("#5CCDC9");
            pictureBox2.BackColor = ColorTranslator.FromHtml("#1D7471");
            pictureBox1.BackColor = ColorTranslator.FromHtml("#009B95");

            Connectbutton.BackColor = ColorTranslator.FromHtml("#009B95");
            Voicbutton.BackColor = ColorTranslator.FromHtml("#A64A00");
            Webbutton.BackColor = ColorTranslator.FromHtml("#A64A00");

            toolStrip1.BackColor = ColorTranslator.FromHtml("#1D7471");
            label4.ForeColor = ColorTranslator.FromHtml("#5CCDC9");
            label4.BackColor = ColorTranslator.FromHtml("#1D7471");
            toolStripDropDownButton1.BackColor = ColorTranslator.FromHtml("#009B95");
            камераToolStripMenuItem.BackColor = ColorTranslator.FromHtml("#009B95");
            микрофонToolStripMenuItem.BackColor = ColorTranslator.FromHtml("#009B95");
            label3.BackColor = ColorTranslator.FromHtml("#1D7471");
            label3.ForeColor = ColorTranslator.FromHtml("#5CCDC9");

            label2.BackColor = ColorTranslator.FromHtml("#009B95");
            label2.ForeColor = Color.White;
            label1.ForeColor = Color.White;
            label1.BackColor = ColorTranslator.FromHtml("#1D7471");
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                videocap.SignalToStop();
                videocap.WaitForStop();
                Application.Exit();

            }
            catch { }
        }
        bool Stop = true;
        private void Webbutton_Click(object sender, EventArgs e)
        {
            try
            {
                if (Stop)
                {
                    label2.Visible = false;
                    videocap = new VideoCaptureDevice(filcol[SelectCam].MonikerString);
                    cs.ToAddr = IPAddress.Parse(textBox3.Text);
                    cs.ToPort = 32188;
                    videocap.NewFrame += Videoframe;
                    videocap.Start();
                    Stop = false;
                    Webbutton.BackgroundImage = Image.FromFile("вкл камера.png");
                }
                else
                {
                    videocap.Stop();
                    videocap.SignalToStop();
                    Stop = true;
                    Webbutton.BackgroundImage = Image.FromFile("выкл камера.png");
                }
            }
            catch {  }
        }

        bool flagCon = false;
        private void Voicbutton_Click(object sender, EventArgs e)
        {
            try
            {
                
                if (flagCon)
                {
                    recorder.StopRecording();
                    Voicbutton.BackgroundImage = Image.FromFile("выкл.png");
                    flagCon = false;
                }
                else
                {
                    

                        cs.ToAddrAud = IPAddress.Parse(textBox3.Text);
                        cs.ToPortAud = 32189;
                 
                        Voicbutton.BackgroundImage = Image.FromFile("вкл.png");
                        //listBox1.Items.Add(filcol1[0].MonikerString);
                        recorder = new WaveIn();
                        recorder.DeviceNumber = SelectMic;
                
                    //определяем его формат - частота дискретизации 8000 Гц, ширина сэмпла - 16 бит, 1 канал - моно
                    recorder.WaveFormat = new WaveFormat(8000, 16, 1);
                    //добавляем код обработки нашего голоса, поступающего на микрофон
                    recorder.DataAvailable += cs.Voice_Input;
                    //создаем поток для прослушивания входящего звука                
                    recorder.StartRecording();
                    flagCon = true ;
                }
                
            }
           
            catch (Exception ex)
            { Voicbutton.BackgroundImage = Image.FromFile("выкл.png");
               // MessageBox.Show(ex.Message);
            }
        }


      



        private void timer1_Tick(object sender, EventArgs e)
        {
        
        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {
         
        }

        private void камераToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Stop = true;
                videocap.Stop();
                videocap.SignalToStop();
                Webbutton.BackgroundImage = Image.FromFile("выкл камера.png");

            }
            catch { }
            VideoFilter vf = new VideoFilter();
                vf.ShowDialog();
            
        }

        private void микрофонToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Stop = true;
                recorder.StopRecording();
                recorder.Dispose();
                Voicbutton.BackgroundImage = Image.FromFile("выкл.png");
                flagCon = false;
            }
            catch { }
            VoiceFilter vf = new VoiceFilter();
            vf.ShowDialog();
        }

        private void pictureBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {   if (pictureBox1.Size == new Size(s.Width/2,s.Height/2))
            {
                pictureBox1.Size = new Size(167, 126);
                pictureBox1.Location = new Point(27, 338);
                
            }
            else
            {

                pictureBox1.Size = new Size(s.Width / 2, s.Height / 2);
                pictureBox1.Location = new Point(Size.Width/3, Size.Height / 3);
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
          
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
    
        }

        private void pictureBox2_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void Connectbutton_MouseMove(object sender, MouseEventArgs e)
        {
            Connectbutton.BackColor = ColorTranslator.FromHtml("#33CDC7");
        }

        private void Connectbutton_MouseLeave(object sender, EventArgs e)
        {
            Connectbutton.BackColor = ColorTranslator.FromHtml("#009B95");
        }

        private void Webbutton_MouseMove(object sender, MouseEventArgs e)
        {
            Webbutton.BackColor = ColorTranslator.FromHtml("#33CDC7");

        }

        private void Webbutton_MouseLeave(object sender, EventArgs e)
        {
            Webbutton.BackColor = ColorTranslator.FromHtml("#009B95");
        }

        private void Voicbutton_MouseMove(object sender, MouseEventArgs e)
        {
            Voicbutton.BackColor = ColorTranslator.FromHtml("#33CDC7");
        }

        private void Voicbutton_MouseLeave(object sender, EventArgs e)
        {
            Voicbutton.BackColor = ColorTranslator.FromHtml("#009B95");
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

        private void toolStripDropDownButton1_MouseMove(object sender, MouseEventArgs e)
        {
            toolStripDropDownButton1.BackColor = ColorTranslator.FromHtml("#5CCDC9");
        }

        private void toolStripDropDownButton1_MouseLeave(object sender, EventArgs e)
        {
            toolStripDropDownButton1.BackColor = ColorTranslator.FromHtml("#009B95");
        }

        private void label4_Click(object sender, EventArgs e)
        {
            Start st = new Start();
            st.Show();
            this.Close();
         
        }

        private void label3_MouseMove(object sender, MouseEventArgs e)
        {
            label3.ForeColor = ColorTranslator.FromHtml("#FFB173");
        }

        private void label3_MouseLeave(object sender, EventArgs e)
        {
            label3.ForeColor = ColorTranslator.FromHtml("#5CCDC9");
        }

        private void label3_MouseDown(object sender, MouseEventArgs e)
        {
            label3.ForeColor = ColorTranslator.FromHtml("#FF7100");
        }
        bool r = true;
        private void label3_Click(object sender, EventArgs e)
        {
            if (r)
            {
                this.WindowState = FormWindowState.Maximized;
                r = false;
                label1.Location = new Point(Size.Width / 3, Size.Height / 3);
          
            }
            else
            {
                this.WindowState = FormWindowState.Normal;
                r = true;
                label1.Location = new Point(119, 203);
            }
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
