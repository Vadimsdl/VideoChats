using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AForge.Video;
using AForge.Video.DirectShow;
using FM.IceLink.WebRTC;
using FM.IceLink.WebRTC.NAudio;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using NAudio.Wave;
using System.Drawing.Imaging;

namespace Client
{
    class ConnectServer
    {
        public static int myPort;
        public Queue<string> messageQueue = new Queue<string>();
       
        UdpClient client,client2;

        public IPEndPoint myEpVideo { get; set; } // адрес получателя
        public IPEndPoint myEpAudio { get; set; }
        public IPAddress ToAddr { get; set; }
        public IPAddress ToAddrAud { get; set; }
        public int ToPort { get; set; }
        public int ToPortAud { get; set; }

        public byte[] Byt { set; get; }
     

        Thread timer2; // поток приема
        Thread timer1;


        public ConnectServer()
        {  
        }

        public void OtpravkaVid()
        {
            try
            {
                FileStream fs = new FileStream("qqwwe.jpg", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);

          
                int readed = 0;

               
        
                byte[] bit=new byte[5000];
                fs.Write(Byt, 0, Byt.Length);
                fs.Position = 0;
                do
                    {                   
                    readed = fs.Read(bit, 0, 5000);             
                    sendBlock(bit, readed);               
                    }
                    while (readed > 0);
                fs.Close();
            
                byte[] dgram1 = Encoding.Default.GetBytes("@@@The End@@@");
                using (UdpClient server = new UdpClient())
                {
                    server.Send(dgram1, dgram1.Length, new IPEndPoint(ToAddr, ToPort));
                }
              
            }
            catch (Exception ex) { //MessageBox.Show(ex.Message+" Otpravkavid");
            }
        }

        public void Voice_Input(object sender, WaveInEventArgs e)
        {
            try
            {
                using (UdpClient server = new UdpClient())
                {
                    server.Send(e.Buffer, e.Buffer.Length, new IPEndPoint(ToAddrAud, ToPortAud));
                }
            }
            catch (Exception ex)
            {
               // MessageBox.Show(ex.Message);
            }
        }     

        private void Listening()
        {
            //Прослушиваем по адресу
            IPEndPoint localIP = myEpAudio;                     
            BufferedWaveProvider bufferedWaveProvider = new BufferedWaveProvider(new WaveFormat(8000, 16, 1));
            // WaveOut player = new WaveOut();
            DirectSoundOut player = new DirectSoundOut();
                    do
                    {
                        try
                        {

                            byte[] receiveBytes = client2.Receive(ref localIP);
                            //добавляем данные в буфер, откуда output будет воспроизводить звук

                            bufferedWaveProvider.AddSamples(receiveBytes, 0, receiveBytes.Length);
                
                    player.Init(bufferedWaveProvider);
                         
                    player.Play();
                            //player.Dispose();
                        }
                        catch (Exception ex)
                        {            
                          //   MessageBox.Show(ex.Message);                          
                        }
                    } while (true);          
        }

        void sendBlock(byte[] block, int size)
        {
            using (UdpClient server = new UdpClient())
            {
                server.Send(block, size, new IPEndPoint(ToAddr, ToPort));
            }
        }

        List<string> fileArr = new List<string>();
        // асинхронный метод
        void priem()
        {
            try
            {
                bool Flag = false;
                IPEndPoint cep = myEpVideo;
                string quest = "";
                int count = 0;
                do
                {
                    string filename = Path.GetTempFileName() + ".jpg";
                    FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.Write);
                    
                    fileArr.Add(filename);
                    do
                    {

                        byte[] receiveBytes = client.Receive(ref cep);

                        try
                        {
                            quest = Encoding.Default.GetString(receiveBytes);

                        }
                        catch { }

                        if (quest != "@@@The End@@@" || receiveBytes.Length != 0)
                            fs.Write(receiveBytes, 0, receiveBytes.Length);

                        if (quest == "@@@The End@@@")
                        {

                            if (fs.Length != 0)
                            {

                                Flag = true;
                                break;

                            }

                        }



                    } while (true);
                    fs.Flush();
                    fs.Close();



                    if (Flag)
                        Form1.self.pictureBox2.BackgroundImage = Image.FromFile(filename);
                 
                } while (true);              
            }
            catch (Exception ex)
            {
               // myEpVideo.Address = null;
               // myEpAudio.Address = null;
               // MessageBox.Show(ex.Message + ex.Source + ex.StackTrace + ex.TargetSite.Name+ "poluchenie");
          
            }

        }

        public void startExchangeVid()
        {
            try
            {

                client = new UdpClient(myEpVideo);



                timer1 = new Thread(priem);

                timer1.IsBackground = true;
                //timer1.Start();                          
                timer1.Start();               
            
                
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }

        public void Stop()
        {
            client.Close();
            client2.Close();
            
        }

        public void startExchangeAud()
        {
            try
            {

                client2 = new UdpClient(myEpAudio);



                timer2 = new Thread(Listening);

                timer2.IsBackground = true;
                timer2.Start();
       
          
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }

    }
}