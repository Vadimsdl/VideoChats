using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;

namespace Client
{
    public partial class Start : Form
    {
        public Start()
        {
            InitializeComponent();
        }

        private void Start_Load(object sender, EventArgs e)
        {
            this.BackColor = ColorTranslator.FromHtml("#5CCDC9");
            linkLabel1.ActiveLinkColor = ColorTranslator.FromHtml("#BF6F30");
            label1.ForeColor = ColorTranslator.FromHtml("#006561");
            label2.ForeColor = Color.White;
            label3.ForeColor = Color.White;
            button1.ForeColor = ColorTranslator.FromHtml("#33CDC7");
            linkLabel1.LinkColor = ColorTranslator.FromHtml("#1D7471");
            button1.BackColor = ColorTranslator.FromHtml("#006561");
            textBox1.BackColor = ColorTranslator.FromHtml("#1D7471");
            panel1.BackColor = ColorTranslator.FromHtml("#1D7471");
            label4.ForeColor = ColorTranslator.FromHtml("#5CCDC9");
            String host = System.Net.Dns.GetHostName();
            // Получение ip-адреса.
            System.Net.IPAddress ip = System.Net.Dns.GetHostByName(host).AddressList[0];
            // Показ адреса в label'е.
            linkLabel1.Text = ip.ToString();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;
            e.Handled = c >= 'а' && c <= 'я' || c >= 'А' && c <= 'Я' || c == 'ё' || c == 'Ё' || c >= 'A' && c<='Z'|| c>='a' && c<='z' || c=='/' ||c==',' || c == ' ' || c =='`';
            if (!e.Handled) return;
            else
                e.Handled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                IPEndPoint ip = new IPEndPoint(IPAddress.Parse(textBox1.Text), 5325);
                //if (textBox1.Text.Length >= 15 || textBox1.Text.Length <= 8 || textBox1.Text.IndexOf('.')<=2 || textBox1.Text.IndexOf('.') >= 4)
                //{
                //    return;
                //}
                //else
                //{
                    
                    Form1 f = new Form1();
                    f.textBox3.Text = textBox1.Text;
                    f.Show();
                   this.Hide() ;
                //}
            }
            catch {  }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Clipboard.SetText(linkLabel1.Text);
            NI.BalloonTipText = "Текст: " + linkLabel1.Text;
            NI.BalloonTipTitle = "Фрагмент, сохраненный в буфере обмена";
            NI.BalloonTipIcon = ToolTipIcon.Info;
            NI.Icon = this.Icon;
            NI.Visible = true;
            NI.ShowBalloonTip(1000);
           
        }

        private void button1_MouseMove(object sender, MouseEventArgs e)
        {

            button1.BackColor = ColorTranslator.FromHtml("#009B95");
        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
            button1.BackColor = ColorTranslator.FromHtml("#006561");
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox1.BackColor = ColorTranslator.FromHtml("#009B95");
            textBox1.ForeColor = ColorTranslator.FromHtml("#5CCDC9");
        }

        private void label4_MouseMove(object sender, MouseEventArgs e)
        {
            label4.ForeColor = ColorTranslator.FromHtml("#FFB173");
        }

        private void label4_MouseLeave(object sender, EventArgs e)
        {
            label4.ForeColor = ColorTranslator.FromHtml("#5CCDC9");
        }

        private void label4_Click(object sender, EventArgs e)
        {
            Application.Exit();
            this.Close();
        }

        private NotifyIcon NI = new NotifyIcon();
   

        private void NI_BalloonTipClosed(Object sender, EventArgs e)
        {
            NI.Visible = false;
        }
        private void label4_MouseDown(object sender, MouseEventArgs e)
        {
            label4.ForeColor = ColorTranslator.FromHtml("#FF7100");
        }

        Point p;

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
                    
                p = new Point(e.X, e.Y);
   
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Left += e.X - p.X;
                Top += e.Y - p.Y;
            }
        }
    }
}
