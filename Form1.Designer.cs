
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web;

using System.ComponentModel;
using System.Data;
using System.Drawing;

using System.Text;

using System.Runtime.InteropServices;  //必须要添加该引用

using System.IO;

using System.Threading;  //导入命名空间,类Thread就在此空间中

using System.Net;
using System.Security.Cryptography;
using System.Xml;
using System.Collections;
using Newtonsoft.Json;//
                      //using NAudio.Wave;
using System.Net.WebSockets;
using Newtonsoft.Json.Linq;//
using MACRODEFIND;
//using proctransmsg;


namespace translatetool
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        public enum enTransType
        {
            NONE_TYPE,
            CHINESE_TYPE,
            ENGLISH_TYPE,
            RUSSIAN_TYPE
        };

        public class CLargeFileInfo
        {
            public  List<byte> currentdata = new List<byte>();
            public  List<byte> translatedata = new List<byte>();
        };
        [DllImport("kernel32")]
        static extern uint GetTickCount();
        private delegate int TSDKInitCallback(CDefineinfo.enSdkCbType eCbType, IntPtr pParam, ulong dwSize, int usr_data);
        public static int g_nPcmTotalLen1 = 0;
        public static Stream chinesefile = null;
        public static Stream englishfile = null;
        public static Stream russianfile = null;
        public static bool exitprogram;
        public static string strdata;
        public static TimeSpan chinesetimeSpan;
        public static uint chinesetimeSpan1;
        public static TimeSpan englishtimeSpan;
        public static uint englishtimeSpan1;
        public static TimeSpan russiantimeSpan;
        public static uint russiantimeSpan1;
        public static enTransType clicktype;
        public static int timeout;
        public static bool nextsend;
        public static int translatecuridx;
        public static string presure;
        public static List<CLargeFileInfo> listlargedata = new List<CLargeFileInfo>();
        public string devstatus;
        public string curlanguage;
        public static FileStream tmpFile;
        public static bool isinittmpFile;
        public static  FileStream TextFile;
        public static FileStream LargeTextFile;
        public static FileStream TranslateTextFile;

        string transappId = "";
        string transsecretKey = "";

        string regappid = "";
        string regsecret = "";
        string regapikey = "";

         string x_appid = "";
        // 接口密钥（webapi类型应用开通听写服务后，控制台--我的应用---语音听写---相应服务的apikey）
         string api_secret = "";
        // 接口密钥（webapi类型应用开通听写服务后，控制台--我的应用---语音听写---相应服务的apisecret）
        //const string api_key = "d2292487531d829b044a41728e371351";
         string api_key = "";

        //public static List<byte *> listbytedata ;

        public static Mutex m = new Mutex();
        public static Mutex listmutex = new Mutex();

        public static Mutex largefileListmutex = new Mutex();
        public static Mutex translatelistmutex = new Mutex();
        public static Mutex englishtranslatelistmutex = new Mutex();

        public static Mutex reconnectmutex = new Mutex();

        public static Mutex russiantranslatelistmutex = new Mutex();
        public static AutoResetEvent chineseEvent;
        public static AutoResetEvent englishEvent;
        public static AutoResetEvent russianEvent;
        public static AutoResetEvent LargeFileEvent;
        public static AutoResetEvent chinesenextEvent;

        public static AutoResetEvent sndnextEvent;
        public static AutoResetEvent recvnextEvent;
        public static AutoResetEvent recvennextEvent;

        public static AutoResetEvent englishnextEvent;
        public static AutoResetEvent russiannextEvent;
        public static AutoResetEvent largefilenextEvent;
        public static AutoResetEvent translateEvent;
        public static AutoResetEvent russiantranslateEvent;
        public static AutoResetEvent englishtranslateEvent;
        public static List<List<byte> > chineselistdata = new List<List<byte> >();
        public static List<List<byte>> englishlistdata = new List<List<byte>>();
        public static List<List<byte>> russianlistdata = new List<List<byte>>();

        public static List<List<char>> chinesetranslistdata = new List<List<char>>();
        public static List<List<char>> englishtranslistdata = new List<List<char>>();

        public static List<byte> translatelisttdata = new List<byte>();
        public static List<byte> englishtranslatelisttdata = new List<byte>();
        public static List<byte> russiantranslatelisttdata = new List<byte>();

        public static AutoResetEvent sndEvent;

        public static void WriteFile(string data)
        {
            Encoding encoder = Encoding.UTF8;
            byte []Info = encoder.GetBytes(data);
            TextFile.Write(Info, 0, Info.Length);
            TextFile.Flush(true);
        }

        public static void WriteLargeFile(string data)
        {
            Encoding encoder = Encoding.UTF8;
            byte[] Info = encoder.GetBytes(data);
            LargeTextFile.Write(Info, 0, Info.Length);
            LargeTextFile.Flush(true);
        }

        public static void WriteTranslateFile(string data)
        {
            Encoding encoder = Encoding.UTF8;
            byte[] Info = encoder.GetBytes(data);
            TranslateTextFile.Write(Info, 0, Info.Length);
            TranslateTextFile.Flush(true);
        }

        private static string GetTermType(CDefineinfo.enSdkDevType eTermType)
        {
            switch (eTermType)
            {
                case CDefineinfo.enSdkDevType.TSDK_DEV_TYPE_TALK:
                    return "[对讲广播终端]";
                case CDefineinfo.enSdkDevType.TSDK_DEV_TYPE_BROAD:
                    return "[广播终端]";
                case CDefineinfo.enSdkDevType.TSDK_DEV_TYPE_MP3:
                    return "[网络拾音器]";
            }

            return "[未知类型终端]";
        }

        private static string GetStateText(CDefineinfo.enSdkDevState eState)
        {
            switch (eState)
            {
                case CDefineinfo.enSdkDevState.TSDK_DEV_STATE_OFFLINE:
                    return "不在线";
                case CDefineinfo.enSdkDevState.TSDK_DEV_STATE_ONLINE:
                    return "在线";
                case CDefineinfo.enSdkDevState.TSDK_DEV_STATE_PLAYING:
                    return "播放音乐";
                case CDefineinfo.enSdkDevState.TSDK_DEV_STATE_TALKING:
                    return "通话中";
                case CDefineinfo.enSdkDevState.TSDK_DEV_STATE_TALK_PLAY:
                    return "通话中并播放音乐";
            }

            return "未知状态";
        }



        [DllImport("CtsSdk.dll", EntryPoint = "TSDK_Init", CallingConvention = CallingConvention.Cdecl)]  //最关键的，导入该dll
        private static extern int TSDK_Init(TSDKInitCallback callback, bool bServerMode, int usr_data); // SDK 初始化。

        [DllImport("CtsSdk.dll", EntryPoint = "TSDK_DeInit", CallingConvention = CallingConvention.Cdecl)]  //最关键的，导入该dll
        public static extern int TSDK_DeInit(); // SDK 初始化。

        private static byte[] CreateWaveFileHeader(int data_Len, int data_SoundCH, int data_Sample, int data_SamplingBits)
        {
            // WAV音频文件头信息
            List<byte> WAV_HeaderInfo = new List<byte>();  // 长度应该是44个字节
            WAV_HeaderInfo.AddRange(Encoding.ASCII.GetBytes("RIFF"));           // 4个字节：固定格式，“RIFF”对应的ASCII码，表明这个文件是有效的 "资源互换文件格式（Resources lnterchange File Format）"
            WAV_HeaderInfo.AddRange(BitConverter.GetBytes(data_Len + 38));  // 4个字节：总长度-8字节，表明从此后面所有的数据长度，小端模式存储数据
            WAV_HeaderInfo.AddRange(Encoding.ASCII.GetBytes("WAVE"));           // 4个字节：固定格式，“WAVE”对应的ASCII码，表明这个文件的格式是WAV
            WAV_HeaderInfo.AddRange(Encoding.ASCII.GetBytes("fmt "));           // 4个字节：固定格式，“fmt ”(有一个空格)对应的ASCII码，它是一个格式块标识
            WAV_HeaderInfo.AddRange(BitConverter.GetBytes(18));                 // 4个字节：fmt的数据块的长度（如果没有其他附加信息，通常为16），小端模式存储数据
            var fmt_Struct = new
            {
                PCM_Code = (short)1,                  // 4B，编码格式代码：常见WAV文件采用PCM脉冲编码调制格式，通常为1。
                SoundChannel = (short)1,   // 2B，声道数
                SampleRate = (int)16000,        // 4B，没个通道的采样率：常见有：11025、22050、44100等
                BytesPerSec = (int)(16000 * 2),  // 4B，数据传输速率 = 声道数×采样频率×每样本的数据位数/8。播放软件利用此值可以估计缓冲区的大小。
                BlockAlign = (short)(2),               // 2B，采样帧大小 = 声道数×每样本的数据位数/8。
                SamplingBits = (short)16,     // 4B，每个采样值（采样本）的位数，常见有：4、8、12、16、24、32
                cbsize = (short)0,
            };
            // 依次写入fmt数据块的数据（默认长度为16）
            WAV_HeaderInfo.AddRange(BitConverter.GetBytes(fmt_Struct.PCM_Code));
            WAV_HeaderInfo.AddRange(BitConverter.GetBytes(fmt_Struct.SoundChannel));
            WAV_HeaderInfo.AddRange(BitConverter.GetBytes(fmt_Struct.SampleRate));
            WAV_HeaderInfo.AddRange(BitConverter.GetBytes(fmt_Struct.BytesPerSec));
            WAV_HeaderInfo.AddRange(BitConverter.GetBytes(fmt_Struct.BlockAlign));
            WAV_HeaderInfo.AddRange(BitConverter.GetBytes(fmt_Struct.SamplingBits));
            WAV_HeaderInfo.AddRange(BitConverter.GetBytes(fmt_Struct.cbsize));
            /* 还 可以继续写入其他的扩展信息，那么fmt的长度计算要增加。*/

            WAV_HeaderInfo.AddRange(Encoding.ASCII.GetBytes("data"));             // 4个字节：固定格式，“data”对应的ASCII码
            WAV_HeaderInfo.AddRange(BitConverter.GetBytes(data_Len));             // 4个字节：正式音频数据的长度。数据使用小端模式存放，如果是多声道，则声道数据交替存放。
            /* 到这里文件头信息填写完成，通常情况下共44个字节*/
            return WAV_HeaderInfo.ToArray();
        }

        public static void SetKeep()
        {
            
            //GC.KeepAlive(Form1.callbackinfo);
            return;
        }

        //static string path = @".//test.wav";//测试文件路径,自己修改

        public static void SetVisible()
        {
            Form1.chinese.Visible = true;
            Form1.english.Visible = true;
            Form1.Russian.Visible = true;
            Form1.devoff.Visible = false;
            int nResult = TSDK_Req_OpenTermPcm(65541); // 请求打开终端实时音频视频。
            //int nResult = TSDK_Req_OpenTermAudVid(65541);
            //string str = string.Format("chinese_Click:TSDK_Req_OpenTermPcm nResult={0}\n", (int)nResult);
            //byte []Info = encoder.GetBytes(str);
            //TextFile.Write(Info, 0, Info.Length)
        }

        static unsafe int CSCallbackFunction(CDefineinfo.enSdkCbType eCbType, IntPtr pParam, ulong dwSize, int usr_data)
        {

            string str = string.Format("CSCallbackFunction: {0}\r\n", (int)eCbType);
            //WriteFile(str);
            
            CDefineinfo.TSdkDataTermMp3L* pDataTermMp3L;


            //FileStream TextFile = File.Open(@".\translationlog.txt", FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            
            switch (eCbType)
            {
                case CDefineinfo.enSdkCbType.CB_Event_TermRegister:
                    CDefineinfo.TSdkEventTermRegister pEventTermRegister = (CDefineinfo.TSdkEventTermRegister)Marshal.PtrToStructure(pParam, typeof(CDefineinfo.TSdkEventTermRegister));

                    //string tmp = string.Format("{0:X}", pEventTermRegister.dwTermID);
                    //string str = "===> " + GetTermType(pEventTermRegister.eTermType) + " 请求注册：终端ID:{} " + tmp;

                    //IntPtr p = pEventTermRegister.TermMac;
                    break;
                case CDefineinfo.enSdkCbType.CB_Event_TermConnect:
                    CDefineinfo.TSdkEventTermConnect pEventTermConnect = (CDefineinfo.TSdkEventTermConnect)Marshal.PtrToStructure(pParam, typeof(CDefineinfo.TSdkEventTermConnect));
                    return 0;

                case CDefineinfo.enSdkCbType.CB_Post_TermState:
                    CDefineinfo.TSdkPostTermState pPostTermState = (CDefineinfo.TSdkPostTermState)Marshal.PtrToStructure(pParam, typeof(CDefineinfo.TSdkPostTermState));
                    //Console.WriteLine("终端ID:" + pPostTermState.dwTermID + " 终端状态:" + GetStateText(pPostTermState.eTermState));
                    
                    str = "ternibal ID=:" + pPostTermState.dwTermID + " status=" + GetStateText(pPostTermState.eTermState)+"\n";
                    WriteFile(str);


                    if (CDefineinfo.enSdkDevState.TSDK_DEV_STATE_ONLINE == pPostTermState.eTermState)
                    {
                        int ret = TSDK_Req_OpenTermPcm((int)pPostTermState.dwTermID);
                        

                        FileStream TextFile = File.Open(@".\isonline.txt", FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                        Encoding encoder = Encoding.UTF8;
                        byte[] Info = encoder.GetBytes("1\n");
                        TextFile.Write(Info, 0, Info.Length);
                        TextFile.Close();

                       
                        
                        str = " TSDK_Req_OpenTermAudVid ret=" + ret +"\n";
                        WriteFile(str);
                        //Form1.
                        //Form1.chinese.Visible = true;
                        /*if (Form1.chinese.InvokeRequired)
                        {
                            Action SetText = delegate { SetVisible(); };
                            Form1.chinese.Invoke(SetText);
                            GC.KeepAlive(Form1.chinese);
                        }
                        else
                        {
                            Form1.chinese.Visible = true;
                        }*/
                    }
                    //TSdkPostTermState* pPostTermState = (TSdkPostTermState*)pParam;
                    break;



                case CDefineinfo.enSdkCbType.CB_Asw_OpenTermAudVid:
                    {
                        CDefineinfo.TSdkAswOpenTermAudVid pAswOpenTermAudVid = (CDefineinfo.TSdkAswOpenTermAudVid)Marshal.PtrToStructure(pParam, typeof(CDefineinfo.TSdkAswOpenTermAudVid));
                        Console.WriteLine("CB_Asw_OpenTermAudVid nResult=" + pAswOpenTermAudVid.nResult);
                        break;
                    }

                //case enSdkCbType.CB_Data_TermMp3R:
                //System.out.println("____________________");
                //break;

                case CDefineinfo.enSdkCbType.CB_Data_TermMp3L:
                {
                        if (clicktype != enTransType.CHINESE_TYPE || true == exitprogram)
                            return 0;
                        void* p3 = (void*)pParam;
                        pDataTermMp3L = (CDefineinfo.TSdkDataTermMp3L*)p3;
                        var convertedArray2 = new byte[pDataTermMp3L->nDataSize];
                        System.Runtime.InteropServices.Marshal.Copy(new IntPtr(pDataTermMp3L->pMp3Data), convertedArray2, 0, pDataTermMp3L->nDataSize);
                        //Form1.listtmpdata.AddRange(convertedArray1);
                        //file.Write(convertedArray2, 0, convertedArray2.Length);
                        
                        tmpFile.Write(convertedArray2, 0, convertedArray2.Length);
                        tmpFile.Flush(true);
                        
                        break;
                }

                case CDefineinfo.enSdkCbType.CB_Data_TermPcmL:
                    
                    if ((clicktype != enTransType.CHINESE_TYPE && clicktype != enTransType.ENGLISH_TYPE && clicktype != enTransType.RUSSIAN_TYPE) || true == exitprogram)
                        return 0;
                    void* p2 = (void*)pParam;
                    pDataTermMp3L = (CDefineinfo.TSdkDataTermMp3L*)p2;
                    var convertedArray1 = new byte[pDataTermMp3L->nDataSize];
                    System.Runtime.InteropServices.Marshal.Copy(new IntPtr(pDataTermMp3L->pMp3Data), convertedArray1, 0, pDataTermMp3L->nDataSize);
                    TimeSpan curtimeSpan;
                    curtimeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1));
                    int diff = 0;
                    uint diff1 = 0;
                    /*Stream  chinesefile1 = File.Open("tetata.wav", FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                    chinesefile1.Write(convertedArray1, 0, convertedArray1.Length);
                    chinesefile1.Close();*/
                    uint curtimeSpan1 = GetTickCount();

                    if (enTransType.CHINESE_TYPE == clicktype)
                    {
                        chinesefile.Write(convertedArray1, 0, convertedArray1.Length);
                        diff = (int)curtimeSpan.TotalSeconds - (int)chinesetimeSpan.TotalSeconds;
                        diff1 = curtimeSpan1 - chinesetimeSpan1;
                        timeout = 400;
                    }
                    else if (enTransType.ENGLISH_TYPE == clicktype)
                    {
                        englishfile.Write(convertedArray1, 0, convertedArray1.Length);
                        diff = (int)curtimeSpan.TotalSeconds - (int)englishtimeSpan.TotalSeconds;
                        diff1 = curtimeSpan1 - englishtimeSpan1;
                        timeout = 400;
                    }
                        
                    else if (enTransType.RUSSIAN_TYPE == clicktype)
                    {
                        russianfile.Write(convertedArray1, 0, convertedArray1.Length);
                        diff = (int)curtimeSpan.TotalSeconds - (int)russiantimeSpan.TotalSeconds;
                        diff1 = curtimeSpan1 - russiantimeSpan1;
                        timeout = 2000;
                    }
                    
                    g_nPcmTotalLen1 += convertedArray1.Length;
                    strdata += System.Text.Encoding.ASCII.GetString(convertedArray1);

                    /*if (diff >= timeout)
                    {
                        chinesefile1 = File.Open("testttt.wav", FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                        chinesefile1.Seek(0, SeekOrigin.Begin);
                        byte[] WAV_HEADER1 = CreateWaveFileHeader(g_nPcmTotalLen1, 0, 0, 0);
                        chinesefile1.Write(WAV_HEADER1, 0, WAV_HEADER1.Length); ;
                        chinesefile1.Close();
                        Thread.Sleep(200000);
                    
                    +

                    }
                    return 0;*/
                    
                        if (diff1 >= timeout)
                    {
                        
                        if (strdata.Length > 0)
                        {
                            //byte[] WAV_HEADER1 = CreateWaveFileHeader(strdata.Length, 0, 0, 0);
                            string strpath = "";
                            if (enTransType.CHINESE_TYPE == clicktype)
                            {
                                chinesefile.Seek(0, SeekOrigin.Begin);
                                byte[] WAV_HEADER1 = CreateWaveFileHeader(g_nPcmTotalLen1, 0, 0, 0);
                                chinesefile.Write(WAV_HEADER1, 0, WAV_HEADER1.Length); ;
                                chinesefile.Close();
                              // Thread.Sleep(200000);
                                strpath = chinesepath;


                            }
                            else if (enTransType.ENGLISH_TYPE == clicktype)
                            {
                                englishfile.Seek(0, SeekOrigin.Begin);
                                byte[] WAV_HEADER1 = CreateWaveFileHeader(g_nPcmTotalLen1, 0, 0, 0);
                                englishfile.Write(WAV_HEADER1, 0, WAV_HEADER1.Length); ;
                                englishfile.Close();
                                strpath = englishpath;
                                //Thread.Sleep(200000);
                            }

                            else if (enTransType.RUSSIAN_TYPE == clicktype)
                            {
                                russianfile.Close();
                                strpath = russianpath;
                                //Thread.Sleep(200000);
                            }
                            
                            //删除文件
                            //Event.Set();

                            var AudioData = File.ReadAllBytes(strpath);
                            List<byte> lsttest = new List<byte>();
                            for (int i = 0; i < AudioData.Length; i++)
                            {
                                lsttest.Add(AudioData[i]);
                            }

                           
                            
                            str = "CSCallbackFunction:put data to list" + "\n";
                            WriteFile(str);

                            if (enTransType.CHINESE_TYPE == clicktype)
                            {
                                listmutex.WaitOne();
                                chineselistdata.Add(lsttest);
                                listmutex.ReleaseMutex();

                                if (File.Exists(chinesepath))
                                    File.Delete(chinesepath);

                                chinesefile = new FileStream(chinesepath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                               
                                byte[] chinese_WAV_HEADER = CreateWaveFileHeader(0, 0, 0, 0);
                                chinesefile.Write(chinese_WAV_HEADER, 0, chinese_WAV_HEADER.Length);
                                chineseEvent.Set();
                                //Thread.Sleep(200000);
                            }
                            else if (enTransType.ENGLISH_TYPE == clicktype)
                            {
                                listmutex.WaitOne();
                                englishlistdata.Add(lsttest);
                                listmutex.ReleaseMutex();
                                if (File.Exists(englishpath))
                                    File.Delete(englishpath);

                                englishfile = new FileStream(englishpath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                                
                                byte[] chinese_WAV_HEADER = CreateWaveFileHeader(0, 0, 0, 0);
                                englishfile.Write(chinese_WAV_HEADER, 0, chinese_WAV_HEADER.Length);
                                englishEvent.Set();
                            }

                            else if (enTransType.RUSSIAN_TYPE == clicktype)
                            {
                                listmutex.WaitOne();
                                russianlistdata.Add(lsttest);
                                listmutex.ReleaseMutex();
                                if (File.Exists(russianpath))
                                    File.Delete(russianpath);

                                russianfile = new FileStream(russianpath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                                russianEvent.Set();
                            }

                           
                            //TextFile = File.Open(@".\translationlog.txt", FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                            
                                
                            g_nPcmTotalLen1 = 0;
                            
                            //Thread.Sleep(200000);
                        }
                        else
                        {
                            
                            str = "CSCallbackFunction:dont have data" + "\n";
                            WriteFile(str);


                        }

                        if (enTransType.CHINESE_TYPE == clicktype)
                        {
                            chinesetimeSpan = curtimeSpan;
                            chinesetimeSpan1 = curtimeSpan1;

                        }
                        else if (enTransType.ENGLISH_TYPE == clicktype)
                        {
                            englishtimeSpan = curtimeSpan;
                            englishtimeSpan1 = curtimeSpan1;

                        }

                        else if (enTransType.RUSSIAN_TYPE == clicktype)
                        {
                            russiantimeSpan = curtimeSpan;
                            russiantimeSpan1 = curtimeSpan1;
                        }

                        
                        //
                        strdata = "";
                        g_nPcmTotalLen1 = 0;
                        //strdata += ;
                    }
                    else
                    {
                        //str = "CSCallbackFunction len=" + strdata.Length + " g_nPcmTotalLen1=" + g_nPcmTotalLen1 + " current=" + pDataTermMp3L->nDataSize + " diff= " + diff + " timeout=" + timeout + "\n";
                        //Info = encoder.GetBytes(str);

                    }
                   
                   

                    break;
                case CDefineinfo.enSdkCbType.CB_Data_TermPcmR:

                    break;

            };
            return 0;
        }

        

        [DllImport("CtsSdk.dll", EntryPoint = "TSDK_Req_OpenTermPcm", CallingConvention = CallingConvention.Cdecl)]  //最关键的，导入该dll
        public static extern int TSDK_Req_OpenTermPcm(int dwTermID); // 请求打开终端实时音频视频。

        [DllImport("CtsSdk.dll", EntryPoint = "TSDK_Req_OpenTermAudVid", CallingConvention = CallingConvention.Cdecl)]  //最关键的，导入该dll
        public static extern int TSDK_Req_OpenTermAudVid(int dwTermID); // 请求打开终端实时音频视频。

        static TSDKInitCallback callbackinfo = CSCallbackFunction;

        #region Windows 窗体设计器生成的代码

        ToolStripMenuItem AddContextMenu(string text, ToolStripItemCollection cms, EventHandler callback)
        {
            if (text == "-")
            {
                ToolStripSeparator tsp = new ToolStripSeparator();
                cms.Add(tsp);
                return null;
            }
            else if (!string.IsNullOrEmpty(text))
            {
                ToolStripMenuItem tsmi = new ToolStripMenuItem(text);
                tsmi.Tag = text + "TAG";
                if (callback != null) tsmi.Click += callback;
                cms.Add(tsmi);

                return tsmi;
            }

            return null;
        }

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.KeyPreview = true;
            clicktype = enTransType.NONE_TYPE;
            //timeout = 1;
            timeout = 400;
            strdata = "";
            g_nPcmTotalLen1 = 0;

            string path = "paraConfig.xml";

            //创建xml实例对象
            XmlDocument xmldoc = new XmlDocument();

            try
            {
                if (File.Exists(@".\isonline.txt"))
                {
                    File.Delete(@".\isonline.txt");
                }
                xmldoc.Load(path);
                //获取portConfigure配置节点
                XmlNode paraConfigure = xmldoc.SelectSingleNode("Configure/paraConfigure");
                //读取节点数据
                presure = paraConfigure.SelectSingleNode("pypath").InnerText;

                transappId = paraConfigure.SelectSingleNode("transappId").InnerText;
                transsecretKey = paraConfigure.SelectSingleNode("transsecretKey").InnerText;


                x_appid = paraConfigure.SelectSingleNode("regappid").InnerText;
                api_secret = paraConfigure.SelectSingleNode("regsecret").InnerText;
                api_key = paraConfigure.SelectSingleNode("regapikey").InnerText;



                //MessageBox.Show(presure);
                /*string load = paraConfigure.SelectSingleNode(textBox2.Tag.ToString()).InnerText;
                string rate = paraConfigure.SelectSingleNode(textBox3.Tag.ToString()).InnerText;
                string distance = paraConfigure.SelectSingleNode(textBox4.Tag.ToString()).InnerText;

                textBox1.Text = presure;
                textBox2.Text = load;
                textBox3.Text = rate;
                textBox4.Text = distance;*/

                //xmldoc.Save(path);

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }

            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.setting = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem7 = new System.Windows.Forms.ToolStripMenuItem();
            this.originaltext = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setting});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(3, 1, 0, 1);
            this.menuStrip1.Size = new System.Drawing.Size(1676, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // setting
            // 
            this.setting.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem3,
            this.toolStripMenuItem4,
            this.toolStripMenuItem5,
            this.toolStripMenuItem6,
            this.toolStripMenuItem7});
            this.setting.Name = "setting";
            this.setting.Size = new System.Drawing.Size(44, 22);
            this.setting.Text = "设置";
            this.setting.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.setting.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(124, 22);
            this.toolStripMenuItem3.Text = "中文(ctrl+z)";
            this.toolStripMenuItem3.Click += new System.EventHandler(this.toolStripMenuItem3_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(124, 22);
            this.toolStripMenuItem4.Text = "英文(ctrl+e)";
            this.toolStripMenuItem4.Click += new System.EventHandler(this.toolStripMenuItem4_Click);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(124, 22);
            this.toolStripMenuItem5.Text = "俄文(ctrl+r)";
            this.toolStripMenuItem5.Click += new System.EventHandler(this.toolStripMenuItem5_Click);
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(124, 22);
            this.toolStripMenuItem6.Text = "放大字体(ctrl+i)";
            this.toolStripMenuItem6.Click += new System.EventHandler(this.toolStripMenuItem6_Click);
            // 
            // toolStripMenuItem7
            // 
            this.toolStripMenuItem7.Name = "toolStripMenuItem7";
            this.toolStripMenuItem7.Size = new System.Drawing.Size(124, 22);
            this.toolStripMenuItem7.Text = "缩小字体(ctrl+d)";
            this.toolStripMenuItem7.Click += new System.EventHandler(this.toolStripMenuItem7_Click);
            // 
            // originaltext
            // 
            this.originaltext.Location = new System.Drawing.Point(-2, 0);
            this.originaltext.Name = "originaltext";
            this.originaltext.Size = new System.Drawing.Size(1900, 450);
            this.originaltext.TabIndex = 1;
            //this.originaltext.Text = "label17777777777777777777777777777777777";
            // 
            // panel3
            // 
            this.panel3.AutoScroll = true;
            this.panel3.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.panel3.Controls.Add(this.originaltext);
            this.panel3.Location = new System.Drawing.Point(24, 27);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1900, 450);
            this.panel3.TabIndex = 2;
            // 
            // panel4
            // 
            this.panel4.AutoScroll = true;
            this.panel4.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.panel4.Controls.Add(this.label2);
            this.panel4.Location = new System.Drawing.Point(12, 500);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(1900, 450);
            this.panel4.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(-2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(1900, 450);
            this.label2.TabIndex = 1;
            //this.label2.Text = "label17777777777777777777777777777777777";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1676, 696);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MainMenuStrip = this.menuStrip1;

            if (File.Exists(chinesepath))
            {
                File.Delete(chinesepath);
            }

            clicktype = enTransType.CHINESE_TYPE;
            chinesetimeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1));
            chinesetimeSpan1 = GetTickCount();
            chinesefile = File.Open(chinesepath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            byte[] chinese_WAV_HEADER = CreateWaveFileHeader(0, 0, 0, 0);
            chinesefile.Write(chinese_WAV_HEADER, 0, chinese_WAV_HEADER.Length);

            this.Name = "Form1";
            devstatus = "设备离线";
            curlanguage = "中文-";
            this.Text = curlanguage +devstatus;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

            TextFile = File.Open(@".\translationlog.txt", FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            LargeTextFile = File.Open(@".\largefilelog.txt", FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            TranslateTextFile = File.Open(@".\translationres.txt", FileMode.Append, FileAccess.Write, FileShare.ReadWrite);

            tmpFile = File.Open(@".\tmpFile.mp3", FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            isinittmpFile = false;

            //file = new FileStream("test.wav", FileMode.OpenOrCreate);
            //byte[] chinese_WAV_HEADER = CreateWaveFileHeader(0, 0, 0, 0);
            //file.Write(chinese_WAV_HEADER, 0, chinese_WAV_HEADER.Length);

            int nResult = TSDK_Init(Form1.callbackinfo, true, 0x12345678);
            //GC.KeepAlive(callbackinfo);

            sndnextEvent = new AutoResetEvent(false);
            recvnextEvent = new AutoResetEvent(false);

            recvennextEvent = new AutoResetEvent(false);

            chineseEvent = new AutoResetEvent(false);
            russianEvent = new AutoResetEvent(false);
            englishEvent = new AutoResetEvent(false);
            LargeFileEvent = new AutoResetEvent(false);
            largefilenextEvent = new AutoResetEvent(false);
            chinesenextEvent = new AutoResetEvent(false);
            englishnextEvent = new AutoResetEvent(false);
            russiannextEvent = new AutoResetEvent(false);
            translateEvent = new AutoResetEvent(false);
            russiantranslateEvent = new AutoResetEvent(false);
            englishtranslateEvent = new AutoResetEvent(false);
            sndEvent = new AutoResetEvent(false);
            webSocket0 = new ClientWebSocket();

             timeStamp = StringUtil.GetTimeStamp();
             md5Code = GetMd5Code(x_appid + timeStamp);
             hmacsha1Code = HMACSHA1Text(md5Code, api_key);

            hmacsha1Code = StringUtil.UrlEncode(hmacsha1Code);
            if (webSocket0.State == WebSocketState.Open)
            {
                //WriteFile("sndmsgthd: after webSocket0.ConnectAsync succeed\n");
                webSocket0.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, cancellation);
            }


            //webSocket0 = new ClientWebSocket();
            webSocket0.ConnectAsync(new Uri(wsUrl + "?lang=cn&appid=" + x_appid + "&ts=" + timeStamp + "&signa=" + hmacsha1Code), cancellation).Wait();
            //WriteFile("sndmsgthd: dont ****  send data will send message len=" + lsttest.ToArray().Length + " status=" + webSocket0.State + "\n");
            if (webSocket0.State == WebSocketState.Open)
                WriteFile("init.ConnectAsync succeed state=" + webSocket0.State + "\n");
            else
                WriteFile("init webSocket0.ConnectAsync failure state=" + webSocket0.State + "\n");

            Thread thread22 = new Thread(new ParameterizedThreadStart(sndmsgthd));
            thread22.Start();

            Thread thread1111 = new Thread(new ParameterizedThreadStart(showmsgthd));
            thread1111.Start();

            Thread thread1 = new Thread(new ParameterizedThreadStart(sndtranslatethd));
            thread1.Start();

            Thread thread2 = new Thread(new ParameterizedThreadStart(sndenglishmsgthd));
            thread2.Start();

            Thread thread21 = new Thread(new ParameterizedThreadStart(showenmsgthd));
            thread21.Start();

            Thread thread4 = new Thread(new ParameterizedThreadStart(sndenglishtranslatethd));
            thread4.Start();

            Thread thread6 = new Thread(new ParameterizedThreadStart(sndRussianmsgthd));
            thread6.Start();

            Thread thread3 = new Thread(new ParameterizedThreadStart(sndrussiantranslatethd));
            thread3.Start();

            translatecuridx = 0;
            Thread largethread = new Thread(new ParameterizedThreadStart(sndlargemsgthd));
            //largethread.Start();

         

            Thread status = new Thread(new ParameterizedThreadStart(chkdevstatusthd));
            status.Start();

            CheckForIllegalCrossThreadCalls = false;

            


        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            //ctrl+f 查找
            if (e.Control & e.KeyCode == Keys.Z)//ctrl+f  
            {
                //MessageBox.Show("Z");
                //MessageBox.Show("中文");
                if (chinesefile != null)
                    return;
                if (File.Exists(chinesepath))
                {
                    /*using (var chinesefile = new FileStream(chinesepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        chinesefile.Close();
                    }*/

                    File.Delete(chinesepath);
                }

                clicktype = enTransType.CHINESE_TYPE;
                chinesetimeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1));
                chinesetimeSpan1 = GetTickCount();
                chinesefile = File.Open(chinesepath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                byte[] chinese_WAV_HEADER = CreateWaveFileHeader(0, 0, 0, 0);
                chinesefile.Write(chinese_WAV_HEADER, 0, chinese_WAV_HEADER.Length);
                //这里写你摁下Ctrl+F之后所要进行的动作的代码

            }
            else if (e.Control & e.KeyCode == Keys.E)//ctrl+f  
            {
                //MessageBox.Show("E");
                if (englishfile != null)
                    return;
                //MessageBox.Show("英文");
                if (File.Exists(englishpath))
                    File.Delete(englishpath);

                clicktype = enTransType.ENGLISH_TYPE;
                englishtimeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1));
                englishtimeSpan1 = GetTickCount();
                englishfile = File.Open(englishpath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                byte[] chinese_WAV_HEADER = CreateWaveFileHeader(0, 0, 0, 0);
                englishfile.Write(chinese_WAV_HEADER, 0, chinese_WAV_HEADER.Length);
                //这里写你摁下Ctrl+F之后所要进行的动作的代码

            }
            else if (e.Control & e.KeyCode == Keys.R)//ctrl+f  
            {
                //MessageBox.Show("R");
                if (russianfile != null)
                    return;
                if (File.Exists(russianpath))
                    File.Delete(russianpath);

                //MessageBox.Show("俄文");
                clicktype = enTransType.RUSSIAN_TYPE;
                russiantimeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1));
                russiantimeSpan1 = GetTickCount();
                russianfile = File.Open(russianpath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                //这里写你摁下Ctrl+F之后所要进行的动作的代码

            }
            else if (e.Control & e.KeyCode == Keys.I)//ctrl+f  
            {
                //MessageBox.Show("I");
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
                //这里写你摁下Ctrl+F之后所要进行的动作的代码

            }
            else if (e.Control & e.KeyCode == Keys.D)//ctrl+f  
            {
                MessageBox.Show("D");
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
                //这里写你摁下Ctrl+F之后所要进行的动作的代码

            }
        }

        private void chkdevstatusthd(object message)
        {
            while (true)
            {
                if (File.Exists(@".\isonline.txt"))
                {
                    string str = File.ReadAllText(@".\isonline.txt");//第一种
                    str = str.Replace("\r", "");
                    str = str.Replace("\n", "");
                    
                    if (str == "1")
                    {
                        devstatus = "设备在线";
                        curlanguage = "中文-";
                        this.Text = curlanguage + devstatus;
                        //this.Text = devstatus;
                        break;
                    }
                }
                
               
                Thread.Sleep(1000);
            }
            
        }

         List<string> arrdata = new List<string>();

         private ResponseData GetResultData1(string ReceviceStr)
        {
            
                ResponseData temp = new ResponseData();
            if (clicktype != enTransType.CHINESE_TYPE)
                return temp;
            ReaponseDataInfo dataInfo = new ReaponseDataInfo();
            ResponseResultInfo resultInfo = new ResponseResultInfo();

            int i = 1;
            //WriteFile("ReceviceStr=" + ReceviceStr + "\n");
            var jsonObj = (JObject)JsonConvert.DeserializeObject(ReceviceStr);

            string totalstr = "";
            //WriteFile("showmsgthd: after reLength=" + reLength + "\n");
            WriteFile("action=" + jsonObj["action"].ToObject<string>() + "\n");
            
            if (jsonObj["action"].ToObject<string>() != "result")
                return temp;

            string result_1 = jsonObj["action"].ToObject<string>();
            string tmpdata = jsonObj["data"].ToObject<string>();
            WriteFile("tmpdata=" + tmpdata+"\n");
            //Console.WriteLine("tmpdata=" + tmpdata);
            var jsonObj1 = (JObject)JsonConvert.DeserializeObject(tmpdata);
            string type = jsonObj1["cn"]["st"]["type"].ToObject<string>();
            Console.WriteLine("type=" + type);
            WriteFile("type=" + type);
            JArray rtArray = jsonObj1["cn"]["st"]["rt"].ToObject<JArray>();
            JArray wsArray;
            JArray cwArray;

            for (i = 0; i < rtArray.Count; i++)
            {
                wsArray = rtArray[i]["ws"].ToObject<JArray>();
                for (int j = 0; j < wsArray.Count; j++)
                {
                    cwArray = wsArray[j]["cw"].ToObject<JArray>();
                    for (int k = 0; k < cwArray.Count; k++)
                    {
                        totalstr += cwArray[k]["w"].ToObject<string>();
                    }
                }
            }

            if (type == "1")
                i = 1;
            else
            {
                arrdata.Add(totalstr);
                totalstr = "";
            }

            tmpdata = "";

            for (int j = 0; j < arrdata.Count; j++)
            {
                tmpdata += arrdata[j];
            }

            tmpdata = tmpdata + totalstr;
            originaltext.Text = tmpdata;//lgh
            List<char> lsttest = new List<char>();
            for (int h = 0; h < tmpdata.Length; h++)
            {
                lsttest.Add(tmpdata[h]);
            }
            WriteFile("GetResultData1: tmpdata=" + tmpdata + "\n");
            Form1.translatelistmutex.WaitOne();
            chinesetranslistdata.Add(lsttest);
            //Form1.translatelisttdata.AddRange(System.Text.Encoding.Default.GetBytes(tmpdata));
            Form1.translatelistmutex.ReleaseMutex();
            //Form1.translateEvent.Set();//lgh
            //Console.WriteLine("tmpdata=" + tmpdata);
            return temp;
        }

        private ResponseData GetResultData2(string ReceviceStr)
        {
            ResponseData temp = new ResponseData();
            
            if (clicktype != enTransType.ENGLISH_TYPE)
                return temp;
            ReaponseDataInfo dataInfo = new ReaponseDataInfo();
            ResponseResultInfo resultInfo = new ResponseResultInfo();

            int i = 1;
            //WriteFile("ReceviceStr=" + ReceviceStr + "\n");
            var jsonObj = (JObject)JsonConvert.DeserializeObject(ReceviceStr);

            string totalstr = "";
            //WriteFile("showmsgthd: after reLength=" + reLength + "\n");
            WriteFile("action=" + jsonObj["action"].ToObject<string>() + "\n");

            if (jsonObj["action"].ToObject<string>() != "result")
                return temp;

            string result_1 = jsonObj["action"].ToObject<string>();
            string tmpdata = jsonObj["data"].ToObject<string>();
            WriteFile("tmpdata=" + tmpdata + "\n");
            //Console.WriteLine("tmpdata=" + tmpdata);
            var jsonObj1 = (JObject)JsonConvert.DeserializeObject(tmpdata);
            string type = jsonObj1["cn"]["st"]["type"].ToObject<string>();
            Console.WriteLine("type=" + type);
            WriteFile("type=" + type);
            JArray rtArray = jsonObj1["cn"]["st"]["rt"].ToObject<JArray>();
            JArray wsArray;
            JArray cwArray;

            for (i = 0; i < rtArray.Count; i++)
            {
                wsArray = rtArray[i]["ws"].ToObject<JArray>();
                for (int j = 0; j < wsArray.Count; j++)
                {
                    cwArray = wsArray[j]["cw"].ToObject<JArray>();
                    for (int k = 0; k < cwArray.Count; k++)
                    {
                        totalstr += cwArray[k]["w"].ToObject<string>();
                    }
                }
            }

            if (type == "1")
                i = 1;
            else
            {
                arrdata.Add(totalstr);
                totalstr = "";
            }

            tmpdata = "";

            for (int j = 0; j < arrdata.Count; j++)
            {
                tmpdata += arrdata[j];
            }

            List<char> lsttest = new List<char>();
            for (int h = 0; h < tmpdata.Length; h++)
            {
                lsttest.Add(tmpdata[h]);
            }
            WriteFile("GetResultData1: tmpdata=" + tmpdata + "\n");
            
            

            tmpdata = tmpdata + totalstr;
            originaltext.Text = tmpdata;//lgh
            WriteFile("GetResultData2: tmpdata=" + tmpdata + "\n");
            englishtranslatelistmutex.WaitOne();
            englishtranslistdata.Add(lsttest);
            //englishtranslatelisttdata.AddRange(System.Text.Encoding.Default.GetBytes(lastdata));
            englishtranslatelistmutex.ReleaseMutex();
            englishtranslateEvent.Set();//lgh
            //Console.WriteLine("tmpdata=" + tmpdata);
            return temp;
        }

        byte[] ReceiveBuff = new byte[1024*1024*100];//根据实际情况设置大小

        async void reconnect(string desc, int flag)
        {
            reconnectmutex.WaitOne();
            timeStamp = StringUtil.GetTimeStamp();
            md5Code = GetMd5Code(x_appid + timeStamp);
            hmacsha1Code = HMACSHA1Text(md5Code, api_key);
            if (webSocket0.State == WebSocketState.Open)
            {
                reconnectmutex.ReleaseMutex();
                return;
            }
                

            if (webSocket0.State != WebSocketState.Aborted)
            {
                webSocket0.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, cancellation);
            }

            if (webSocket0.State != WebSocketState.Open)
            {
                webSocket0 = new ClientWebSocket();
                if (0 == flag)
                    webSocket0.ConnectAsync(new Uri(wsUrl + "?lang=cn&appid=" + x_appid + "&ts=" + timeStamp + "&signa=" + hmacsha1Code), cancellation).Wait();
                else
                    webSocket0.ConnectAsync(new Uri(wsUrl + "?lang=en&appid=" + x_appid + "&ts=" + timeStamp + "&signa=" + hmacsha1Code), cancellation).Wait();
                if (webSocket0.State == WebSocketState.Open)
                    WriteFile(desc+": after webSocket0.ConnectAsync succeed\n");
                else
                    WriteFile(desc+": after webSocket0.ConnectAsync failure\n");
            }
            reconnectmutex.ReleaseMutex();
        }


        async void showmsgthd(object message)
        {

            //Thread.Sleep(5000);
             timeStamp = StringUtil.GetTimeStamp();
             md5Code = GetMd5Code(x_appid + timeStamp);
             hmacsha1Code = HMACSHA1Text(md5Code, api_key);

            hmacsha1Code = StringUtil.UrlEncode(hmacsha1Code);
            while (true)
            {
                try
                {
                    //recvnextEvent.WaitOne(60000000);
                    recvnextEvent.WaitOne();
                    if (webSocket0.State == WebSocketState.Open)
                    {
                        if (clicktype != enTransType.CHINESE_TYPE)
                        {
                            string str1 = string.Format("showmsgthd: return type=" + clicktype + " not correct");
                            WriteFile(str1);

                            continue;
                        }

                        var receive = await webSocket0.ReceiveAsync(new ArraySegment<byte>(ReceiveBuff), cancellation);
                        if (receive.MessageType != WebSocketMessageType.Close)
                        {
                            int reLength = receive.Count;
                            WriteFile("showmsgthd: reLength=" + reLength + "\n");
                            if (reLength > 0)
                            {
                                var reData = SubArray(ReceiveBuff, 0, reLength);
                                var ReceviceStr = System.Text.Encoding.UTF8.GetString(reData);
                                WriteFile("showmsgthd: after reLength=" + reLength + "\n");
                                string suffix = "}";

                                bool endsWith = ReceviceStr.EndsWith(suffix);
                                //WriteFile("showmsgthd1: ReceviceStr=" + ReceviceStr + "\n");
                                if (ReceviceStr.Substring(0, 1) == "{" && true == endsWith)
                                {
                                    Console.WriteLine("true");        // True
                                    GetResultData1(ReceviceStr);
                                }
                                //GetResultData1(ReceviceStr);
                            }
                        }
                        else
                        {
                            WriteFile("showmsgthd: recv error ****  MessageType=" + receive.MessageType + "\n");
                            reconnect("showmsgthd", 0);
                           
                        }
                        //sndnextEvent.Set();
                    }
                    else
                    {
                        WriteFile("showmsgthd: recv 11 error ****  MessageType="+"\n");
                        reconnect("showmsgthd", 0);
                    }
                    //Thread.Sleep(500);
                }
                catch (Exception x)
                {
                    reconnect("showmsgthd", 0);
                    WriteFile("showmsgthd: err=" + x.Message+"\n");
                    Console.WriteLine($"Got exception: {x}");
                }
                //

                // receive = webSocket0.ReceiveAsync(new ArraySegment<byte>(ReceiveBuff), cancellation);
                //reLength = receive.Result.Count;
            }
        }

        async void showenmsgthd(object message)
        {

            //Thread.Sleep(5000);
            timeStamp = StringUtil.GetTimeStamp();
            md5Code = GetMd5Code(x_appid + timeStamp);
            hmacsha1Code = HMACSHA1Text(md5Code, api_key);

            hmacsha1Code = StringUtil.UrlEncode(hmacsha1Code);
            while (true)
            {
                try
                {
                    recvennextEvent.WaitOne();
                    if (webSocket0.State == WebSocketState.Open)
                    {

                        var receive = await webSocket0.ReceiveAsync(new ArraySegment<byte>(ReceiveBuff), cancellation);
                        if (receive.MessageType != WebSocketMessageType.Close)
                        {
                            int reLength = receive.Count;
                            WriteFile("showenmsgthd: reLength=" + reLength + "\n");
                            if (reLength > 0)
                            {
                                if (clicktype != enTransType.ENGLISH_TYPE)
                                {
                                    string str1 = string.Format("showenmsgthd: return type=" + clicktype + " not correct");
                                    WriteFile(str1);

                                    continue;
                                }
                                var reData = SubArray(ReceiveBuff, 0, reLength);
                                var ReceviceStr = System.Text.Encoding.UTF8.GetString(reData);
                                WriteFile("showenmsgthd: after reLength=" + reLength + "\n");
                                string suffix = "}";

                                bool endsWith = ReceviceStr.EndsWith(suffix);
                                //WriteFile("showmsgthd1: ReceviceStr=" + ReceviceStr + "\n");
                                if (ReceviceStr.Substring(0, 1) == "{" && true == endsWith)
                                {
                                    Console.WriteLine("true");        // True
                                    GetResultData2(ReceviceStr);
                                }
                                //GetResultData1(ReceviceStr);
                            }
                        }
                        else
                        {
                            WriteFile("showenmsgthd: error ****** MessageType=" + receive.MessageType + "\n");
                            reconnect("showenmsgthd", 1);
                        }
                        //sndnextEvent.Set();
                    }

                    //Thread.Sleep(500);
                }
                catch (Exception x)
                {
                    reconnect("showenmsgthd", 1);
                    WriteFile("showenmsgthd: err=" + x.Message + "\n");
                    
                }
                //

                // receive = webSocket0.ReceiveAsync(new ArraySegment<byte>(ReceiveBuff), cancellation);
                //reLength = receive.Result.Count;
            }
        }

        public  void Tasker1(byte[] AudioData, Form1 tmpform)
        {
            string timeStamp = StringUtil.GetTimeStamp();
            string md5Code = GetMd5Code(x_appid + timeStamp);
            string hmacsha1Code = HMACSHA1Text(md5Code, api_key);
            hmacsha1Code = StringUtil.UrlEncode(hmacsha1Code);
            WriteFile("Tasker1: func begin");
            try
            {
                //await webSocket0.ConnectAsync(new Uri(wsUrl + "?appid=" + x_appid + "&ts=" + timeStamp + "&signa=" + hmacsha1Code), cancellation);
                //WriteFile("Tasker1: after ConnectAsync\n");


                //连接成功，开始发送数据
                int frameSize = 122 * 8; //每一帧音频的大小,建议每 40ms 发送 122B
                int intervel = 10;
                int status = 0;  // 音频的状态
                //while (true)
               // {
                    //AudioData
                    // var AudioData = File.ReadAllBytes("test_1.pcm");
                    byte[] buffer /*= new byte[frameSize]*/;
                    for (int i = 0; i < AudioData.Length; i += frameSize)
                    {

                        if (clicktype != enTransType.CHINESE_TYPE)
                        {
                            string str1 = string.Format("Tasker1: return type=" + clicktype + " not correct");
                            WriteFile(str1);

                            return;
                        }
                    buffer = SubArray(AudioData, i, frameSize);
                    //await Task.Delay(intervel); //模拟音频采样延时
                    //var frameData = System.Text.Encoding.UTF8.GetBytes(buffer);
                        webSocket0.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, cancellation).Wait();
                        //WriteFile("Tasker1: after webSocket0.SendAsync frameSize=" + frameSize+"\n");
                        Task.Delay(intervel).Wait(); //模拟音频采样延时
                    }

                    WriteFile("Tasker1: call recvnextEvent.Set()" + frameSize + "\n");
                    recvnextEvent.Set();
                    
                    
            }
            catch (Exception e)
            {
                WriteFile("Tasker1: err******=" + e.Message+ " webSocket0.State="+ webSocket0.State+"\n");
                //Console.WriteLine(e.Message);
                reconnect("Tasker1", 0);
            }

            return;
        }

        public  void Tasker2(byte[] AudioData, Form1 tmpform)
        {
            
            WriteFile("Tasker2: func begin");
            try
            {
                //await webSocket0.ConnectAsync(new Uri(wsUrl + "?appid=" + x_appid + "&ts=" + timeStamp + "&signa=" + hmacsha1Code), cancellation);
                //WriteFile("Tasker1: after ConnectAsync\n");


                //连接成功，开始发送数据
                int frameSize = 122 * 8; //每一帧音频的大小,建议每 40ms 发送 122B
                int intervel = 10;
                int status = 0;  // 音频的状态
                                 //while (true)
                                 // {
                                 //AudioData
                                 // var AudioData = File.ReadAllBytes("test_1.pcm");
                byte[] buffer /*= new byte[frameSize]*/;
                for (int i = 0; i < AudioData.Length; i += frameSize)
                {
                    if (clicktype != enTransType.ENGLISH_TYPE)
                    {
                        string str1 = string.Format("Tasker2: return type=" + clicktype + " not correct");
                        WriteFile(str1);

                        return;
                    }
                    buffer = SubArray(AudioData, i, frameSize);
                    //await Task.Delay(intervel); //模拟音频采样延时
                    //var frameData = System.Text.Encoding.UTF8.GetBytes(buffer);
                    webSocket0.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, cancellation).Wait();
                    //WriteFile("Tasker1: after webSocket0.SendAsync frameSize=" + frameSize+"\n");
                    Task.Delay(intervel).Wait(); //模拟音频采样延时
                }

                WriteFile("Tasker2: call recvnextEvent.Set()" + frameSize + "\n");
                recvennextEvent.Set();


            }
            catch (Exception e)
            {
                WriteFile("Tasker2: err******=" + e.Message + " webSocket0.State=" + webSocket0.State + "\n");
                //Console.WriteLine(e.Message);
                reconnect("Tasker2", 1);
            }

            return;
        }

        async private void sndmsgthd(object message)
        {
           
               // webSocket0.ConnectAsync(new Uri(wsUrl + "?lang=cn&appid=" + x_appid + "&ts=" + timeStamp + "&signa=" + hmacsha1Code), cancellation).Wait();
                //WriteFile("sndmsgthd: after webSocket0.ConnectAsync failure\n"); 
                
            while (true)
            {
                
                string str1 = string.Format("sndmsgthd: before Event.WaitOne\n");
                WriteFile(str1);


                chineseEvent.WaitOne(10000);
                 str1 = string.Format("sndmsgthd: after Event.WaitOne\n");
                WriteFile(str1);

                while (true)
                {
                    listmutex.WaitOne();
                    if (clicktype != enTransType.CHINESE_TYPE)
                    {
                        listmutex.ReleaseMutex();
                        //FileStream TextFile;

                        str1 = string.Format("sndmsgthd: type="+ clicktype + " not correct");
                        WriteFile(str1);
                        
                        break;
                    }
                    if (chineselistdata.Count <= 0)
                    {
                        listmutex.ReleaseMutex();
                        //FileStream TextFile;
                        
                        str1 = string.Format("sndmsgthd: dont have data\n");
                        WriteFile(str1);
                        break;
                    }

                    
                    TimeSpan timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1));
                    List<byte> lsttest = new List<byte>();
                    for (int i = 0; i < chineselistdata.First().Count; i++)
                    {
                        lsttest.Add(chineselistdata.First()[i]);
                    }

                    if (File.Exists(@".\chineseFile.wav"))
                    {
                        File.Delete(@".\chineseFile.wav");
                    }

                    FileStream contentfile;
                    contentfile = File.Open(@".\chineseFile.wav", FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                    contentfile.Write(lsttest.ToArray(), 0, lsttest.ToArray().Length);
                    contentfile.Flush(true);
                    contentfile.Close();

                    chineselistdata.RemoveAt(0);
                    listmutex.ReleaseMutex();
                    
                    //System.Threading.Thread.Sleep(100000);
                    //CTransMsg.Tasker(lsttest.ToArray());
                    if (webSocket0.State == WebSocketState.Open)
                    {
                        WriteFile("sndmsgthd: send   will send message len=" + lsttest.ToArray().Length + "\n");
                        Tasker1(lsttest.ToArray(), this);
                        //sndnextEvent.WaitOne(20000);
                    }
                        
                    else
                    {
                        WriteFile("sndmsgthd: error will reconnect"+ "\n");
                        reconnect("sndmsgthd", 0);
                    }
                        
                   


                    TimeSpan curtimeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1));
                    int diff = (int)curtimeSpan.TotalSeconds - (int)timeSpan.TotalSeconds;
                    WriteFile("sndmsgthd: will send message len=" + lsttest.ToArray().Length + "need time=" + diff + "\n");

                }
            }

           
        }

        private void sndenglishmsgthd(object message)
        {
            
            while (true)
            {

                string str1 = string.Format("sndenglishmsgthd: before englishEvent.WaitOne\n");
                WriteFile(str1);


                englishEvent.WaitOne(10000);
                str1 = string.Format("sndenglishmsgthd: after englishEvent.WaitOne\n");
                WriteFile(str1);

                while (true)
                {
                    listmutex.WaitOne();
                    if (clicktype != enTransType.ENGLISH_TYPE)
                    {
                        listmutex.ReleaseMutex();
                        //FileStream TextFile;

                        str1 = string.Format("sndenglishmsgthd: type=" + clicktype + " not correct");
                        WriteFile(str1);

                        break;
                    }
                    if (englishlistdata.Count <= 0)
                    {
                        listmutex.ReleaseMutex();
                        //FileStream TextFile;

                        str1 = string.Format("sndenglishmsgthd: dont have data\n");
                        WriteFile(str1);
                        break;
                    }


                    TimeSpan timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1));
                    List<byte> lsttest = new List<byte>();
                    for (int i = 0; i < englishlistdata.First().Count; i++)
                    {
                        lsttest.Add(englishlistdata.First()[i]);
                    }

                    if (File.Exists(@".\chineseFile.wav"))
                    {
                        File.Delete(@".\chineseFile.wav");
                    }

                    FileStream contentfile;
                    contentfile = File.Open(@".\chineseFile.wav", FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                    contentfile.Write(lsttest.ToArray(), 0, lsttest.ToArray().Length);
                    contentfile.Flush(true);
                    contentfile.Close();

                    englishlistdata.RemoveAt(0);
                    listmutex.ReleaseMutex();
                    WriteFile("sndenglishmsgthd: before will send message len=" + lsttest.ToArray().Length + "\n");
                    //System.Threading.Thread.Sleep(100000);
                    //CTransMsg.Tasker(lsttest.ToArray());
                    if (webSocket0.State == WebSocketState.Open)
                    {
                        WriteFile("sndenglishmsgthd: send   will send message len=" + lsttest.ToArray().Length + "\n");
                        Tasker2(lsttest.ToArray(), this);
                        //sndnextEvent.WaitOne(20000);
                    }

                    else
                    {
                        WriteFile("sndenglishmsgthd: error will reconnect" + "\n");
                        reconnect("sndenglishmsgthd", 1);
                    }

                    TimeSpan curtimeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1));
                    int diff = (int)curtimeSpan.TotalSeconds - (int)timeSpan.TotalSeconds;
                    WriteFile("sndenglishmsgthd: will send message len=" + lsttest.ToArray().Length + "need time=" + diff + "\n");


                    /*EnglishTasker(lsttest.ToArray(), this);
                    bool ret = englishnextEvent.WaitOne(60000);
                    if (false == ret)
                    {
                        WriteFile("sndenglishmsgthd.WaitOne: timeout" + "\n");
                    }

                    TimeSpan curtimeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1));
                    int diff = (int)curtimeSpan.TotalSeconds - (int)timeSpan.TotalSeconds;
                    WriteFile("sndenglishmsgthd: will send message len=" + lsttest.ToArray().Length + "need time=" + diff + "\n");*/

                }
            }


        }

        private  void sndRussianmsgthd(object message)
        {

            while (true)
            {

                string str1 = string.Format("sndRussianmsgthd: before Event.WaitOne\n");
                WriteFile(str1);


                russianEvent.WaitOne(10000);
                str1 = string.Format("sndRussianmsgthd: after Event.WaitOne\n");
                WriteFile(str1);

                while (true)
                {
                    listmutex.WaitOne();
                    if (russianlistdata.Count <= 0 || clicktype != enTransType.RUSSIAN_TYPE)
                    {
                        listmutex.ReleaseMutex();
                        //FileStream TextFile;

                        str1 = string.Format("sndRussianmsgthd: dont have data\n");
                        WriteFile(str1);
                        break;
                    }


                    TimeSpan timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1));
                    List<byte> lsttest = new List<byte>();
                    for (int i = 0; i < russianlistdata.First().Count; i++)
                    {
                        lsttest.Add(russianlistdata.First()[i]);
                    }

                    Encoding encoder = Encoding.UTF8;
                    //byte[] Info = encoder.GetBytes(lsttest.ToArray());

                    if (File.Exists(@".\tmpFile.wav"))
                    {
                        File.Delete(@".\tmpFile.wav");
                    }
                    FileStream contentfile;
                    contentfile = File.Open(@".\tmpFile.wav", FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                    contentfile.Write(lsttest.ToArray(), 0, lsttest.ToArray().Length);
                    contentfile.Flush(true);
                    contentfile.Close();
                    russianlistdata.RemoveAt(0);
                    listmutex.ReleaseMutex();
                    WriteFile("sndRussianmsgthd: before will send message len=" + lsttest.ToArray().Length + "\n");

                    RussianTasker(lsttest.ToArray(), this);
                    bool ret = russiannextEvent.WaitOne(60000);
                    if (false == ret)
                    {
                        WriteFile("nextEvent.WaitOne: timeout" + "\n");
                    }
                    
                    TimeSpan curtimeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1));
                    int diff = (int)curtimeSpan.TotalSeconds - (int)timeSpan.TotalSeconds;
                    WriteFile("sndRussianmsgthd: will send message len=" + lsttest.ToArray().Length + "need time=" + diff + "\n");

                }
            }


        }

        private static void sndlargemsgthd(object message)
        {
            while (true)
            {
                string str1 = string.Format("sndlargemsgthd: before Event.WaitOne\n");
                WriteLargeFile(str1);

                LargeFileEvent.WaitOne(10000);
                str1 = string.Format("sndlargemsgthd: after Event.WaitOne\n");
                WriteLargeFile(str1);

                while (true)
                {
                    largefileListmutex.WaitOne();
                    if (listlargedata.Count <= 0)
                    {
                        largefileListmutex.ReleaseMutex();
                        //FileStream TextFile;
                        
                        str1 = string.Format("sndlargemsgthd: dont have data\n");
                        WriteLargeFile(str1);
                        break;
                    }

                    if (listlargedata.Count- translatecuridx < 2)
                    {
                        largefileListmutex.ReleaseMutex();
                        //FileStream TextFile;
                        break;
                    }

                    

                    TimeSpan timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1));
                    List<byte> lsttest = new List<byte>();

                    for (int i = translatecuridx; i < translatecuridx+2; i++)
                    {
                        for (int j = 0; j < listlargedata[i].currentdata.Count; j++)
                        {
                            lsttest.Add(listlargedata[i].currentdata[j]);
                        }
                    }

                    //listtmpdata.RemoveAt(0);
                    listlargedata.RemoveRange(translatecuridx, 2);
                    largefileListmutex.ReleaseMutex();
                    WriteLargeFile("sndlargemsgthd: before will send message len=" + lsttest.ToArray().Length + "\n");
                    translatecuridx += 2;
                    LargeTasker(lsttest.ToArray());
                    bool ret = largefilenextEvent.WaitOne(60000);
                    if (false == ret)
                    {
                        WriteLargeFile("sndlargemsgthd nextEvent.WaitOne: timeout" + "\n");
                    }
                    else
                    {

                    }
                    TimeSpan curtimeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1));
                    int diff = (int)curtimeSpan.TotalSeconds - (int)timeSpan.TotalSeconds;
                    WriteLargeFile("sndlargemsgthd: will send message len=" + lsttest.ToArray().Length + "need time=" + diff + "\n");

                }
            }


        }

        public static string EncryptString(string str)
        {
            MD5 md5 = MD5.Create();
            // 将字符串转换成字节数组
            byte[] byteOld = Encoding.UTF8.GetBytes(str);
            // 调用加密方法
            byte[] byteNew = md5.ComputeHash(byteOld);
            // 将加密结果转换为字符串
            StringBuilder sb = new StringBuilder();
            foreach (byte b in byteNew)
            {
                // 将字节转换成16进制表示的字符串，
                sb.Append(b.ToString("x2"));
            }
            // 返回加密的字符串
            return sb.ToString();
        }

        private  void sndrussiantranslatethd(object message)
        {
            while (true)
            {
                string str1 = string.Format("sndrussiantranslatethd:  before russiantranslateEvent.WaitOne\n");
                WriteTranslateFile(str1);
                russiantranslateEvent.WaitOne(20000);
                str1 = string.Format("sndrussiantranslatethd:  after russiantranslateEvent.WaitOne\n");
                WriteTranslateFile(str1);

                russiantranslatelistmutex.WaitOne();
                if (russiantranslatelisttdata.Count == 0 ||  clicktype != enTransType.RUSSIAN_TYPE)
                {
                    str1 = string.Format("sndrussiantranslatethd:  dont have data\n");
                    WriteTranslateFile(str1);
                    russiantranslatelistmutex.ReleaseMutex();
                    continue;
                }

                str1 = string.Format("sndrussiantranslatethd:  111\n");
                WriteTranslateFile(str1);
                // 原文
                //string q = "你好";
                // 源语言
                //string from = "en";
                string from = "ru";
                // 目标语言
                string to = "zh";
                //string to = "en";
                // 改成您的APP ID
                string appId = transappId;
                Random rd = new Random();
                string salt = rd.Next(100000).ToString();
                
                string strtmpp = System.Text.Encoding.Default.GetString(russiantranslatelisttdata.ToArray());
                // 改成您的密钥
                string secretKey = transsecretKey;
                
                string sign = EncryptString(appId + strtmpp + salt + secretKey);
                string url = "http://api.fanyi.baidu.com/api/trans/vip/translate?";
                
                url += "q=" + HttpUtility.UrlEncode(strtmpp);
                url += "&from=" + from;
                url += "&to=" + to;
                url += "&appid=" + appId;
                url += "&salt=" + salt;
                url += "&sign=" + sign;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                str1 = string.Format("sndrussiantranslatethd:  after russiantranslateEvent.WaitOne 111\n");
                WriteTranslateFile(str1);
                request.ContentType = "text/html;charset=UTF-8";
                request.UserAgent = null;
                request.Timeout = 10000;
                WriteTranslateFile("sndrussiantranslatethd: url=" + url +  "\n");
                WriteTranslateFile("sndrussiantranslatethd: cnt=" +russiantranslatelisttdata.Count + "\n");
                russiantranslatelisttdata.Clear();
                russiantranslatelistmutex.ReleaseMutex();
                TimeSpan timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1));
                //str1 = string.Format("sndtranslatethd: {1}\n", url);
                
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                string retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();
                WriteTranslateFile("sndrussiantranslatethd: retString111aaa=" + retString + "\n");

                var jsonObj = (JObject)JsonConvert.DeserializeObject(retString);

                JToken token;
                string strres;
                if (jsonObj.Property("error_code") == null || jsonObj.Property("error_code").ToString() == "")
                {
                    if (jsonObj.Property("trans_result") == null || jsonObj.Property("trans_result").ToString() == "")
                    {
                        continue;
                    }

                    var tmp = jsonObj["trans_result"];
                    token = tmp[0]["dst"];
                    strres = (string)token;
                    label2.Text = strres;
                    WriteTranslateFile("sndrussiantranslatethd: strres=" + strres + "\n");
                    TimeSpan curtimeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1));
                    int diff = (int)curtimeSpan.TotalSeconds - (int)timeSpan.TotalSeconds;
                    WriteTranslateFile("sndrussiantranslatethd: will send message" + " need time=" + diff + "\n");
                }
                else
                {
                    token = jsonObj["error_code"];
                    strres = (string)token;

                    WriteTranslateFile("errcode=" + strres + "\n");
                }

            }


        }

        private void sndtranslatethd(object message)
        {
            while (true)
            {
                string str1 = string.Format("sndtranslatethd:  before translateEvent.WaitOne\n");
               // WriteTranslateFile(str1);
                //translateEvent.WaitOne(20000);lgh
                str1 = string.Format("sndtranslatethd:  after translateEvent.WaitOne\n");
               // WriteTranslateFile(str1);

                translatelistmutex.WaitOne();
                
                    if (chinesetranslistdata.Count == 0 || clicktype != enTransType.CHINESE_TYPE)
                {
                    str1 = string.Format("sndtranslatethd:  dont have data\n");
                    //WriteTranslateFile(str1);
                    translatelistmutex.ReleaseMutex();
                    Thread.Sleep(1000);
                    continue;
                }

                str1 = string.Format("sndtranslatethd:  111\n");
                WriteTranslateFile(str1);
                // 原文
                //string q = "你好";
                // 源语言
                //string from = "en";
                string from = "zh";
                // 目标语言
                //string to = "zh";
                string to = "en";
                // 改成您的APP ID
                string appId = transappId;
                Random rd = new Random();
                string salt = rd.Next(100000).ToString();
                //translatelisttdata = "测试";
                string strtmpp = "";
                List<char> lsttest = new List<char>();
                for (int i = 0; i < chinesetranslistdata.Last().Count; i++)
                {
                    strtmpp += chinesetranslistdata.Last()[i];
                    //lsttest.Add(chinesetranslistdata.First()[i]);
                }

                int tmpcnt = chinesetranslistdata.Count;
                chinesetranslistdata.RemoveAt(tmpcnt-1);
                translatelistmutex.ReleaseMutex();
                //string strtmpp = System.Text.Encoding.Default.GetString(lsttest.ToArray());
                // 改成您的密钥
                string secretKey = transsecretKey;
                //string sign = EncryptString(appId + translatelisttdata.ToArray() + salt + secretKey);
                string sign = EncryptString(appId + strtmpp + salt + secretKey);
                string url = "http://api.fanyi.baidu.com/api/trans/vip/translate?";
                //url += "q=" + HttpUtility.UrlEncode(translatelisttdata.ToArray());
                url += "q=" + HttpUtility.UrlEncode(strtmpp);
                url += "&from=" + from;
                url += "&to=" + to;
                url += "&appid=" + appId;
                url += "&salt=" + salt;
                url += "&sign=" + sign;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                str1 = string.Format("sndtranslatethd:  after translateEvent.WaitOne 111\n");
                WriteTranslateFile(str1);
                request.ContentType = "text/html;charset=UTF-8";
                request.UserAgent = null;
                request.Timeout = 20000;

                translatelisttdata.Clear();
                
                TimeSpan timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1));
                //str1 = string.Format("sndtranslatethd: {1}\n", url);
                WriteTranslateFile("sndtranslatethd: url="+url+"\n");
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                string retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();
                WriteTranslateFile("sndtranslatethd1111: retString111aaa=" + retString + "\n");

                var jsonObj = (JObject)JsonConvert.DeserializeObject(retString);
                
                JToken token;
                string strres;
                if (jsonObj.Property("error_code") == null || jsonObj.Property("error_code").ToString() == "")
                {
                    if (jsonObj.Property("trans_result") == null || jsonObj.Property("trans_result").ToString() == "")
                    {
                        continue;
                    }

                    var tmp = jsonObj["trans_result"];
                    token = tmp[0]["dst"];
                    strres = (string)token;
                    string resultB = strres.Replace(", ", "\n");
                    //string resultB = strres.Replace("，", "\n");
                    //this.originaltext.Text = resultB;

                    label2.Text = resultB;
                    //Form1.translatetext.Text = strres;
                    WriteTranslateFile("sndtranslatethd: strres=" + strres + "\n");
                    TimeSpan curtimeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1));
                    int diff = (int)curtimeSpan.TotalSeconds - (int)timeSpan.TotalSeconds;
                    WriteTranslateFile("sndmsgthd: will send message" + " need time=" + diff + "\n");
                }
                else
                {
                    token = jsonObj["error_code"];
                    strres = (string)token;
                
                    WriteTranslateFile("errcode=" + strres + "\n");
                }
                Thread.Sleep(1000);
            }


        }

        private void sndenglishtranslatethd(object message)
        {
            while (true)
            {
                string str1 = string.Format("sndenglishtranslatethd:  before translateEvent.WaitOne\n");
                //WriteTranslateFile(str1);
                //englishtranslateEvent.WaitOne(20000);
                str1 = string.Format("sndenglishtranslatethd:  after translateEvent.WaitOne\n");
                //WriteTranslateFile(str1);

                englishtranslatelistmutex.WaitOne();

               

                if (englishtranslistdata.Count == 0 || clicktype != enTransType.ENGLISH_TYPE)
                {
                    str1 = string.Format("sndenglishtranslatethd:  dont have data\n");
                    //WriteTranslateFile(str1);
                    englishtranslatelistmutex.ReleaseMutex();
                    Thread.Sleep(1000);
                    continue;
                }

                str1 = string.Format("sndenglishtranslatethd:  111\n");
                WriteTranslateFile(str1);
                // 原文
                //string q = "你好";
                // 源语言
                //string from = "en";
                string from = "en";
                // 目标语言
                //string to = "zh";
                string to = "zh";
                // 改成您的APP ID
                string appId = transappId;
                Random rd = new Random();
                string salt = rd.Next(100000).ToString();

                string strtmpp = "";
                List<char> lsttest = new List<char>();
                for (int i = 0; i < englishtranslistdata.Last().Count; i++)
                {
                    strtmpp += englishtranslistdata.Last()[i];
                    //lsttest.Add(chinesetranslistdata.First()[i]);
                }

                int tmpdata = englishtranslistdata.Count();
                englishtranslistdata.RemoveAt(tmpdata-1);
                englishtranslatelistmutex.ReleaseMutex();
                //string strtmpp = System.Text.Encoding.Default.GetString(englishtranslatelisttdata.ToArray());
                // 改成您的密钥
                string secretKey = transsecretKey;
                
                string sign = EncryptString(appId + strtmpp + salt + secretKey);
                string url = "http://api.fanyi.baidu.com/api/trans/vip/translate?";
                
                url += "q=" + HttpUtility.UrlEncode(strtmpp);
                url += "&from=" + from;
                url += "&to=" + to;
                url += "&appid=" + appId;
                url += "&salt=" + salt;
                url += "&sign=" + sign;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                str1 = string.Format("sndenglishtranslatethd:  after translateEvent.WaitOne 111\n");
                WriteTranslateFile(str1);
                request.ContentType = "text/html;charset=UTF-8";
                request.UserAgent = null;
                request.Timeout = 10000;

                //englishtranslatelisttdata.Clear();
                
                TimeSpan timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1));
                //str1 = string.Format("sndtranslatethd: {1}\n", url);
                WriteTranslateFile("sndtranslatethd: url=" + url + "\n");
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                string retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();
                WriteTranslateFile("sndenglishtranslatethd: retString111aaa=" + retString + "\n");

                var jsonObj = (JObject)JsonConvert.DeserializeObject(retString);

                JToken token;
                string strres;
                if (jsonObj.Property("error_code") == null || jsonObj.Property("error_code").ToString() == "")
                {
                    if (jsonObj.Property("trans_result") == null || jsonObj.Property("trans_result").ToString() == "")
                    {
                        continue;
                    }

                    var tmp = jsonObj["trans_result"];
                    token = tmp[0]["dst"];
                    strres = (string)token;
                    label2.Text = strres;
                    //Form1.translatetext.Text = strres;
                    WriteTranslateFile("sndenglishtranslatethd: strres=" + strres + "\n");
                    TimeSpan curtimeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1));
                    int diff = (int)curtimeSpan.TotalSeconds - (int)timeSpan.TotalSeconds;
                    WriteTranslateFile("sndenglishtranslatethd: will send message" + " need time=" + diff + "\n");
                }
                else
                {
                    token = jsonObj["error_code"];
                    strres = (string)token;

                    WriteTranslateFile("errcode=" + strres + "\n");
                }
                Thread.Sleep(1000);
            }

            
        }


        #endregion
        public static System.Windows.Forms.Panel panel1;
        public static System.Windows.Forms.Label originaltext1;
        public static System.Windows.Forms.Button chinese;
        public static System.Windows.Forms.Button Russian;
        public static System.Windows.Forms.Button english;
        public static System.Windows.Forms.Panel panel2;
        public static System.Windows.Forms.Label translatetext;
        public bool chineseclick;
        public bool englishclick;
        public bool russianclick;

        public Color chinesecolor;
        public Color englishcolor;
        public Color russiancolor;
        public static Label devoff;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem setting;
        private ToolStripMenuItem toolStripMenuItem3;
        private ToolStripMenuItem toolStripMenuItem4;
        private ToolStripMenuItem toolStripMenuItem5;
        private ToolStripMenuItem toolStripMenuItem6;
        private ToolStripMenuItem toolStripMenuItem7;
        public  Label originaltext;
        private Panel panel3;
        private Panel panel4;
        public Label label2;
    }
}


public class ResponseData
{
    public int code;
    public string message;
    public string sid;
    public ReaponseDataInfo data;
    public string GetResultText()
    {
        var resultT = data.result;
        var wsT = resultT.ws;
        StringBuilder strB = new StringBuilder();
        for (int i = 0; i < wsT.Length; i++)
        {
            strB.Append(wsT[i].cw[0].w);
        }
        return strB.ToString();
    }
}
public class ReaponseDataInfo
{
    public int status;
    public ResponseResultInfo result;
}
public class ResponseResultInfo
{
    public int bg;
    public int ed;
    public string pgs;
    public int[] rg;
    public int sn;
    public bool ls;
    public Ws[] ws;

}
public class Ws
{
    public Cw[] cw;
    public int bg;
    public int ed;
}
public class Cw
{
    public int sc;
    public string w;
}
