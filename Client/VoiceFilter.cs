using AForge.Video.DirectShow;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class VoiceFilter : Form
    {
        public FilterInfoCollection filcol;
        WaveIn recorder;
        WaveInEvent wvin;
        public BufferedWaveProvider bufferedWaveProvider;

        public WaveOut player;

        public VoiceFilter()
        {
            InitializeComponent();
            ScanSoundCards();

            panel1.BackColor = ColorTranslator.FromHtml("#03899C");
            //Sound();

           
            PlotInitialize();
          
        }
        double[] dataFft;
        private void PlotInitialize()
        {
            if (dataFft != null)
            {
                scottPlotUC1.plt.Clear();
                double fftSpacing = (double)wvin.WaveFormat.SampleRate / dataFft.Length;
                scottPlotUC1.plt.PlotSignal(dataFft, sampleRate: fftSpacing, markerSize: 0);
                scottPlotUC1.plt.PlotHLine(0, color: Color.Black, lineWidth: 1);
                scottPlotUC1.plt.YLabel("Сила увеличения");
                scottPlotUC1.plt.XLabel("Частота (kHz)");
                scottPlotUC1.Render();
            }
        }
        private void AudioMonitorInitialize(
               int DeviceIndex, int sampleRate = 32000,
               int bitRate = 16, int channels = 1,
               int bufferMilliseconds = 50, bool start = true
           )
        {
            if (wvin == null)
            {
                wvin = new NAudio.Wave.WaveInEvent();
                wvin.DeviceNumber = DeviceIndex;
                wvin.WaveFormat = new NAudio.Wave.WaveFormat(sampleRate, bitRate, channels);
                wvin.DataAvailable += OnDataAvailable;
                wvin.BufferMilliseconds = bufferMilliseconds;
                if (start)
                    wvin.StartRecording();
            }
        }

        private void ScanSoundCards()
        {
            comboBox1.Items.Clear();
            for (int i = 0; i < NAudio.Wave.WaveIn.DeviceCount; i++)
                comboBox1.Items.Add(NAudio.Wave.WaveIn.GetCapabilities(i).ProductName);
            if (comboBox1.Items.Count > 0)
                comboBox1.SelectedIndex = 0;
            else
                MessageBox.Show("Нет микрофона");
        }

        Int16[] dataPcm;
        private void OnDataAvailable(object sender, NAudio.Wave.WaveInEventArgs args)
        {
            int bytesPerSample = wvin.WaveFormat.BitsPerSample / 8;
            int samplesRecorded = args.BytesRecorded / bytesPerSample;
            if (dataPcm == null)
                dataPcm = new Int16[samplesRecorded];
            for (int i = 0; i < samplesRecorded; i++)
                dataPcm[i] = BitConverter.ToInt16(args.Buffer, i * bytesPerSample);
        }


        void Sound()
        {
            bufferedWaveProvider = new BufferedWaveProvider(new WaveFormat(8000, 16, 1));
            player = new WaveOut();
            recorder = new WaveIn();

        }

        public void Voice_Input(object sender, WaveInEventArgs e)
        {
            try
            {

               // Sound();
                        player.Play();                                   
                        bufferedWaveProvider.AddSamples(e.Buffer, 0, e.Buffer.Length);
                        //player.Init(bufferedWaveProvider);                                
            }
            catch (Exception ex)
            {
               // MessageBox.Show(ex.Message);
            }
        }

        private void updateFFT()
        {

            // размер PCM для анализа с помощью БПФ должен быть степенью 2
            int fftPoints = 2;
            while (fftPoints * 2 <= dataPcm.Length)
                fftPoints *= 2;
            // применяем оконную функцию Хэмминга при загрузке массива FFT, затем вычисляем FFT
            NAudio.Dsp.Complex[] fftFull = new NAudio.Dsp.Complex[fftPoints];
            for (int i = 0; i < fftPoints; i++)
                fftFull[i].X = (float)(dataPcm[i] * NAudio.Dsp.FastFourierTransform.HammingWindow(i, fftPoints));
            NAudio.Dsp.FastFourierTransform.FFT(true, (int)Math.Log(fftPoints, 2.0), fftFull);
          
            // копируем комплексные значения в двойной массив, который будет построен
            if (dataFft == null)
                dataFft = new double[fftPoints / 2];
            for (int i = 0; i < fftPoints / 2; i++)
            {
                double fftLeft = Math.Abs(fftFull[i].X + fftFull[i].Y);
                double fftRight = Math.Abs(fftFull[fftPoints - i - 1].X + fftFull[fftPoints - i - 1].Y);
                dataFft[i] = fftLeft + fftRight;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            player.Stop();
           
            recorder.StopRecording();
            recorder.Dispose();
            bufferedWaveProvider.ClearBuffer();
            Form1.self.SelectMic = comboBox1.SelectedIndex;
            this.Close();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
      
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox1.BackColor = ColorTranslator.FromHtml("#009B95");
            RT();
        }
        bool flag = true;
        void RT()
        {
            if (flag)
            {

                Sound();
                recorder.DeviceNumber = comboBox1.SelectedIndex;

                recorder.WaveFormat = new WaveFormat(8000,16, 1);

                recorder.DataAvailable += Voice_Input;
        
                player.Init(bufferedWaveProvider);
                recorder.StartRecording();
                flag = false;
                AudioMonitorInitialize(comboBox1.SelectedIndex);
            }
            else {

                if (player != null)
                {
                    player.Stop();
                    player.Dispose();
                    player = null;
                }
                if (recorder != null)
                {
                    recorder.StopRecording();

                    recorder.Dispose();
                   
                    bufferedWaveProvider.ClearBuffer();
                    recorder = null;
                }
                bufferedWaveProvider = null;
                flag = true;
                Sound();
                if (wvin != null)
                {
                    wvin.StopRecording();
                    wvin = null;
                }
                RT();
            }
       }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (dataPcm == null)
                return;

            updateFFT();

            if (scottPlotUC1.plt.GetPlottables().Count == 0)
                PlotInitialize();

            scottPlotUC1.Render();
        }

        private void VoiceFilter_Load(object sender, EventArgs e)
        {
            scottPlotUC1.BackColor = ColorTranslator.FromHtml("#5CCDC9");
                this.BackColor = ColorTranslator.FromHtml("#5CCDC9");
            label4.ForeColor = ColorTranslator.FromHtml("#5CCDC9");
            button1.ForeColor = ColorTranslator.FromHtml("#33CDC7");
            button1.BackColor = ColorTranslator.FromHtml("#006561");
            comboBox1.BackColor = ColorTranslator.FromHtml("#1D7471");
            comboBox1.ForeColor = Color.White;
        }

        private void label4_Click(object sender, EventArgs e)
        {
            try
            {
                player.Stop();

                recorder.StopRecording();
                recorder.Dispose();
                bufferedWaveProvider.ClearBuffer();
                this.Close();
            }
            catch
            {
                this.Close();
            }
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

        private void button1_MouseMove(object sender, MouseEventArgs e)
        {
            button1.BackColor = ColorTranslator.FromHtml("#009B95");
        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
            button1.BackColor = ColorTranslator.FromHtml("#006561");
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
