using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Linq;
using System.Runtime.InteropServices;  //必须要添加该引用

using System.IO;

using System.Threading;  //导入命名空间,类Thread就在此空间中

using System.Net;
using System.Security.Cryptography;

using System.Collections;
using Newtonsoft.Json;//
                      //using NAudio.Wave;
using System.Net.WebSockets;
using Newtonsoft.Json.Linq;//
using MACRODEFIND;

using System.Xml;
//using proctransmsg;

namespace translatetool
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


       
      

        protected override void OnClosing(CancelEventArgs e)
        {
            FileStream TextFile = File.Open(@".\translationlog.txt", FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            //FileStream TextFile = File.Open(@".\translationlog.txt", FileMode.Append, FileShare.ReadWrite);
            Encoding encoder = Encoding.UTF8;
            byte[] Info = encoder.GetBytes("OnClosing: click \n");
            TextFile.Write(Info, 0, Info.Length);
            TextFile.Flush();
            TextFile.Close();


            exitprogram = true;
            
            /*file.Seek(0, SeekOrigin.Begin);

            int nFileLen = (38 + g_nPcmTotalLen1);
            //
            //file.Write(head, 0, head.Length);
            byte[] WAV_HEADER1 = CreateWaveFileHeader(g_nPcmTotalLen1, 0, 0, 0);

            //file.Write(head, 0, head.Length);
            file.Write(WAV_HEADER1, 0, WAV_HEADER1.Length); ; */           

            TSDK_DeInit();
            System.Environment.Exit(0);

        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
           // MessageBox.Show("设置");
        }

        async void reconnect1(string desc, int flag)
        {
            reconnectmutex.WaitOne();
            timeStamp = StringUtil.GetTimeStamp();
            md5Code = GetMd5Code(x_appid + timeStamp);
            hmacsha1Code = HMACSHA1Text(md5Code, api_key);

            arrdata.RemoveRange(0, arrdata.Count);
            if (webSocket0.State != WebSocketState.Aborted)
            {
                WriteFile(desc + ": CloseAsync*********************************\n");
                webSocket0.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, cancellation);
            }
            else
            {
                WriteFile(desc + ": not CloseAsync*********************************\n");
            }
            Thread.Sleep(1000);
            if (webSocket0.State != WebSocketState.Open)
            {
                webSocket0 = new ClientWebSocket();
                if (0 == flag)
                    webSocket0.ConnectAsync(new Uri(wsUrl + "?lang=cn&appid=" + x_appid + "&ts=" + timeStamp + "&signa=" + hmacsha1Code), cancellation).Wait();
                else
                    webSocket0.ConnectAsync(new Uri(wsUrl + "?lang=en&appid=" + x_appid + "&ts=" + timeStamp + "&signa=" + hmacsha1Code), cancellation).Wait();
                if (webSocket0.State == WebSocketState.Open)
                    WriteFile(desc + ": after webSocket0.ConnectAsync succeed\n");
                else
                    WriteFile(desc + ": after webSocket0.ConnectAsync failure\n");
            }
            reconnectmutex.ReleaseMutex();
        }
        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("中文");
            if (clicktype == enTransType.CHINESE_TYPE)
            {
                return;
            }

            try
            {
                using (FileStream fs = File.OpenWrite(chinesepath))
                {
                    //if (fs == null)
                    //  return;
                }
            }
            catch
            {
                chinesefile.Close();
                //Here you know that the file is open by another app
            }


            if (File.Exists(chinesepath))
            {   
                File.Delete(chinesepath);
            }

            
            curlanguage = "中文-";
            this.Text = curlanguage + devstatus;

            
            chinesetimeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1));
            chinesetimeSpan1 = GetTickCount();
            chinesefile = File.Open(chinesepath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            byte[] chinese_WAV_HEADER = CreateWaveFileHeader(0, 0, 0, 0);
            chinesefile.Write(chinese_WAV_HEADER, 0, chinese_WAV_HEADER.Length);
            Form1.lastdata = "";
            originaltext.Text = "";
            label2.Text = "";
           
            
            reconnect1("toolStripMenuItem3_Click", 0);

            originaltext.Text = "";
            clicktype = enTransType.CHINESE_TYPE;
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            if (clicktype == enTransType.ENGLISH_TYPE)
            {
                return;
            }

            try
            {
                using (FileStream fs = File.OpenWrite(englishpath))
                {
                    //if (fs == null)
                    //  return;
                }
            }
            catch
            {
                englishfile.Close();
                //Here you know that the file is open by another app
            }

            //MessageBox.Show("英文");
            if (File.Exists(englishpath))
                File.Delete(englishpath);

            curlanguage = "英文-";
            this.Text = curlanguage + devstatus;
            label2.Text = "";

            clicktype = enTransType.ENGLISH_TYPE;
            englishtimeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1));
            englishtimeSpan1 = GetTickCount();
            englishfile = File.Open(englishpath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            byte[] chinese_WAV_HEADER = CreateWaveFileHeader(0, 0, 0, 0);
            englishfile.Write(chinese_WAV_HEADER, 0, chinese_WAV_HEADER.Length);
            Form1.lastdata = "";
            originaltext.Text = "";
            label2.Text = "";
            originaltext.Text = "";

            reconnect1("toolStripMenuItem4_Click", 1);
            
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            if (clicktype == enTransType.RUSSIAN_TYPE)
            {
                return;
            }

            try
            {
                using (FileStream fs = File.OpenWrite(russianpath))
                {
                    //if (fs == null)
                      //  return;
                }
            }
            catch 
            {
                russianfile.Close();
                //Here you know that the file is open by another app
            }

            

            if (File.Exists(russianpath))
                File.Delete(russianpath);

            //MessageBox.Show("俄文");
            curlanguage = "俄文-";
            originaltext.Text = "";
            this.Text = curlanguage + devstatus;
            clicktype = enTransType.RUSSIAN_TYPE;
            russiantimeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1));
            russiantimeSpan1 = GetTickCount();
            russianfile = File.Open(russianpath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            Form1.lastdata = "";
            originaltext.Text = "";
            label2.Text = "";
            
        }

        private void toolStripMenuItem6_Click(object sender, EventArgs e)
        {
            FontFamily ff = new FontFamily(this.originaltext.Font.Name);
            float size = this.originaltext.Font.Size;
            size += 5;
            FontStyle fontStyle = FontStyle.Regular;
            fontStyle = this.originaltext.Font.Style;
            this.originaltext.Font = new Font(ff, size, fontStyle, GraphicsUnit.World);
            //this.originaltext.Text = "ffffffffff";

            ff = new FontFamily(this.label2.Font.Name);
            float size1 = this.label2.Font.Size;
            size1 += 5;
            fontStyle = FontStyle.Regular;
            fontStyle = this.label2.Font.Style;
            this.label2.Font = new Font(ff, size1, fontStyle, GraphicsUnit.World);
            //this.label2.Text = "222222";

            

            //MessageBox.Show("放大字体");
        }

        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {
            FontFamily ff = new FontFamily(this.originaltext.Font.Name);
            float size = this.originaltext.Font.Size;
            FontStyle fontStyle;
            size -= 5;
            if (size > 0)
            {
                fontStyle = FontStyle.Regular;
                fontStyle = this.originaltext.Font.Style;
                this.originaltext.Font = new Font(ff, size, fontStyle, GraphicsUnit.World);
                //this.originaltext.Text = "ffffffffff";
            }

            ff = new FontFamily(this.label2.Font.Name);
            float size1 = this.label2.Font.Size;
            size1 -= 5;
            if (size1 > 0)
            {

                fontStyle = FontStyle.Regular;
                fontStyle = this.label2.Font.Style;
                this.label2.Font = new Font(ff, size1, fontStyle, GraphicsUnit.World);
                //this.label2.Text = "222222";
            }

            //MessageBox.Show("缩小字体");
        }
    }
}
