using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 任务状态
{
    public partial class Form1 : Form
    {
        [DllImport("GetScreendll.dll", CallingConvention = CallingConvention.Cdecl)]
        extern static bool IsSessionLocked();
        public static Boolean run = true,rebush=false;
        public static float data = 0, data1=0;
        private static FontNum fontNum;
        private static String context = "";
        private static String gp = "sz000519";
        private static int data_i = 0, data1_i = 0;
        public Form1()
        {
            InitializeComponent();
        }
        private string GetHTMLByURLRequest(string URL)
        {
            WebRequest.DefaultWebProxy = null;
            WebRequest request = WebRequest.Create(URL);
            WebResponse resp = request.GetResponse();
            Stream stream = resp.GetResponseStream();
            StreamReader sr = new StreamReader(stream, Encoding.Default);
            string result = sr.ReadToEnd();
            stream.Close();
            sr.Close();
            return result;
        }
        private void DownLoad()
        {
            int count = 100;
            while (run)
            {
                Thread.Sleep(100);
                if (count++ > 100)
                {
                    count = 0;
                    try
                    {
                        //String rest = GetHTMLByURLRequest("http://api.money.126.net/data/feed/0601398%2cmoney.api");
                        String rest = GetHTMLByURLRequest("https://qt.gtimg.cn/q="+ gp);
                        if (context != rest)
                        {
                            context = rest;
                            rebush = true;
                        }
                        //MessageBox.Show(rest,rests[3]);
                    }
                    catch(Exception E)
                    {
                        MessageBox.Show(E.Message);
                        rebush = true;
                        data = 0;
                        data1 = 0;
                    }
                }
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            Thread thread = new Thread(DownLoad);
            thread.Start();
            fontNum = new FontNum(System.AppDomain.CurrentDomain.BaseDirectory+"num16.txt");
            ControlIni.Load(this);
            data_i = comboBox2.SelectedIndex;
            data1_i = comboBox3.SelectedIndex;
            gp = comboBox1.Text;
        }
        private void SetShow1(Color color)
        {
            Bitmap bitmap = new Bitmap(32, 32, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Color bcolor = Color.Transparent;
            if (data1 < 0)
            {
                bcolor = Color.Green;
            }
            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    bitmap.SetPixel(i, j, bcolor);
                }
            }
            fontNum.DrawString(bitmap, data.ToString(".00"), 0, 0, Color.White, bcolor);
            fontNum.DrawString(bitmap, data1.ToString(".00"), 0, 16, Color.White, bcolor);
            panel1.BackgroundImage = bitmap;
            Icon icon = Icon.FromHandle(bitmap.GetHicon());
            this.Icon = icon;
            icon.Dispose();
        }
        private void SetShow(Color color)
        {
            using (Bitmap bitmap1 = new Bitmap(96, 96))
            {
                Color bcolor = Color.Transparent;
                if (data1 < 0)
                {
                    bcolor = Color.Green;
                }
                using (Graphics g = Graphics.FromImage(bitmap1))
                {
                    g.Clear(bcolor);
                    Font font = new Font("Arial", 30, FontStyle.Bold);
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                    g.DrawString(data.ToString(".00"), font, Brushes.White, new PointF(-10, 0));
                    g.DrawString(data1.ToString(".00"), font, Brushes.White, new PointF(-10, 48));
                }
                this.Icon = BitmapToIcon(bitmap1);
                Bitmap bitmap2 = (Bitmap)bitmap1.Clone();
                panel1.BackgroundImage = bitmap2; 
            }
        }

        public static Icon BitmapToIcon(Bitmap bitmap)
        {
            Icon icon = null;
            try
            {
                icon = Icon.FromHandle(bitmap.GetHicon());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error converting bitmap to icon: " + ex.Message);
            }
            return icon;
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            data_i = comboBox2.SelectedIndex;
            label2.Text = data_i.ToString() ;
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            data1_i = comboBox3.SelectedIndex;
            label3.Text = data1_i.ToString() ;
        }

        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            gp=comboBox1.Text;
        }

        private void comboBox1_TextUpdate(object sender, EventArgs e)
        {
         
        }

        internal static FontNum FontNum { get => fontNum; set => fontNum = value; }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            run = false;
            comboBox2.SelectedIndex = data_i;
            comboBox3.SelectedIndex = data1_i;
            ControlIni.Save(this);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if(rebush)
            {
                rebush = false;
                try
                {
                    String[] rests = context.Split('~');
                    for(int i=0;i<rests.Length;i++)
                    {
                        rests[i]=rests[i].Replace("=", ":").Replace("\"", "");
                        rests[i]=rests[i].Replace("\r", "").Replace("\n", "");
                    }
                    if (rests == null) return;
                    if (rests.Length <= 1) return;
                    if (data_i >= rests.Length) return;
                    for (int i = 0; i < rests.Length; i++)
                    {
                        //if (rests[i].IndexOf("0.65") > -1) MessageBox.Show(i.ToString());
                    }
                    comboBox2.Items.Clear();
                    comboBox2.Items.AddRange(rests);
                    comboBox3.Items.Clear();
                    comboBox3.Items.AddRange(rests);
                    comboBox2.AutoCompleteCustomSource.Clear();
                    comboBox2.AutoCompleteCustomSource.AddRange(rests);
                    comboBox3.AutoCompleteCustomSource.Clear();
                    comboBox3.AutoCompleteCustomSource.AddRange(rests);
                    float.TryParse(rests[data_i],out data);
                    float.TryParse(rests[data1_i],out data1);
                    comboBox2.Text = rests[data_i];
                    comboBox3.Text = rests[data1_i];
                    SetShow(Color.Red);
                    this.Text = rests[1];
                    label1.Text =DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss");
                    Boolean ishas = false;
                    for (int i = 0; i < comboBox1.Items.Count; i++)
                    {
                        if (comboBox1.Text == comboBox1.Items[i].ToString()) ishas = true;
                    }
                    if (!ishas) comboBox1.Items.Add(comboBox1.Text);
                }catch(Exception E)
                {
                    this.Text=(E.Message);
                }
                
            }
        }
    }
}
