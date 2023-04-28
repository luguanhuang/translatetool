using System;

using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;  //必须要添加该引用

using System.IO;
using System.Text;
using System.Threading;  //导入命名空间,类Thread就在此空间中
using System.Threading.Tasks;

using translatetool;

using System.Net;
using System.Security.Cryptography;

using System.Collections;
using Newtonsoft.Json;//
					  //using NAudio.Wave;
using System.Net.WebSockets;
using Newtonsoft.Json.Linq;//
using MACRODEFIND;

using System.Diagnostics;
using System.Text.RegularExpressions;
//using WebSocketSharp;


//using System.Runtime.Serialization.Json;

namespace translatetool
{
    partial class Form1
    {
        const int StatusFirstFrame = 0;
        const int StatusContinueFrame = 1;
        const int StatusLastFrame = 2;
        //static FileStream TextFile;
        static ClientWebSocket webSocket0;

        string timeStamp ;
        string md5Code ;
        string hmacsha1Code;

        static CancellationToken cancellation;
        // 应用APPID（必须为webapi类型应用，并开通语音听写服务，参考帖子如何创建一个webapi应用：http://bbs.xfyun.cn/forum.php?mod=viewthread&tid=36481）
        
        // 音频文件地址,示例音频请在听写接口文档底部下载D
        //static string path = @".//en_sentence.mp3";//测试文件路径,自己修改
        static string chinesepath = @".//chinese.wav";//测试文件路径,自己修改
        static string englishpath = @".//english.wav";//测试文件路径,自己修改
        static string russianpath = @".//russian.wav";//测试文件路径,自己修改
        //static string strdata = @".//test.wav";//测试文件路径,自己修改
        static string hostUrl = "https://ws-api.xfyun.cn/v2/iat";
        static string russianres;
        static string chineseres;
        static public string lastdata = "";
        static Form1 tmpformdata;
        //public  Label* label3;
        static void p_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Form1.WriteFile("p_OutputDataReceived: len66=" + "\n");
            if (!string.IsNullOrEmpty(e.Data))
            {
                Form1.WriteFile("p_OutputDataReceived: len77=" + "\n");
                russianres = e.Data;
                //originaltext.Text = "11";
                //lgh
                
                //Console.WriteLine(e.Data);
                //AppendText(e.Data + Environment.NewLine);
            }
        }


        static void p_OutputChineseDataReceived(object sender, DataReceivedEventArgs e)
        {
            Form1.WriteFile("p_OutputChineseDataReceived: len66=" + "\n");
            if (!string.IsNullOrEmpty(e.Data))
            {
                Form1.WriteFile("p_OutputChineseDataReceived: len77=" + "\n");
                chineseres = e.Data;
                
                tmpformdata.originaltext.Text = lastdata + chineseres;
                //lgh

                //Console.WriteLine(e.Data);
                //AppendText(e.Data + Environment.NewLine);
            }
        }

        async public static void RussianTasker(byte[] AudioData, Form1 tmpform)
        {
            //var AudioData = File.ReadAllBytes(path);
            //Form1.SetKeep();
            //Console.WriteLine();

            Form1.WriteFile("len11=" + AudioData.Length + "\n");

            Process p = new Process();
            // p.StartInfo.FileName = @"E:\Program Files (x86)\python\python.exe";//没有配环境变量的话，可以像我这样写python.exe的绝对路径。如果配了，直接写"python.exe"即可
            //p.StartInfo.FileName = @"C:\Users\Administrator\AppData\Local\Programs\Python\Python312-32\python.exe";//没有配环境变量的话，可以像我这样写python.exe的绝对路径。如果配了，直接写"python.exe"即可
            p.StartInfo.FileName = presure;//没有配环境变量的话，可以像我这样写python.exe的绝对路径。如果配了，直接写"python.exe"即可
            
            p.StartInfo.Arguments = "recognize.py";

            p.StartInfo.UseShellExecute = false;

            p.StartInfo.RedirectStandardOutput = true;

            p.StartInfo.RedirectStandardInput = true;

            p.StartInfo.RedirectStandardError = true;

            p.StartInfo.CreateNoWindow = true;

            p.Start();
            Form1.WriteFile("len22=" + AudioData.Length + "\n");
            p.BeginOutputReadLine();
            Form1.WriteFile("len33=" + AudioData.Length + "\n");
            p.OutputDataReceived += new DataReceivedEventHandler(p_OutputDataReceived);
            Form1.WriteFile("len44=" + AudioData.Length + "\n");
            //Console.ReadLine();
            p.WaitForExit();

            string strtmpdata = tmpform.originaltext.Text;


                Form1.WriteFile("strtmpdataaa =" + strtmpdata + "\n");

                string totalstr = strtmpdata + russianres;
            tmpform.originaltext.Text = totalstr;//lgh

                //Form1.listlargedata.currentdata.AddRange(System.Text.Encoding.Default.GetBytes(totalstr));

                Form1.WriteFile("strtmpdatbbb =" + strtmpdata + "\n");
                //Form1.listlargedata.currentdata.AddRange(System.Text.Encoding.Default.GetBytes(totalstr));
                if (russianres != "")
                {
                    Form1.CLargeFileInfo fileinfo = new Form1.CLargeFileInfo();

                    //Form1.listlargedata.Add(fileinfo);
                    //Form1.WriteFile("will add total  totalstr=" + totalstr + "\n");
                   Form1.russiantranslatelistmutex.WaitOne();
                    Form1.russiantranslatelisttdata.AddRange(System.Text.Encoding.Default.GetBytes(totalstr));
                    Form1.russiantranslatelistmutex.ReleaseMutex();
                    Form1.russiantranslateEvent.Set();//lgh
                }

                Form1.WriteFile("strtmpdatbbb =" + strtmpdata + "\n");

                //Form1.russiannextEvent.Set();

                //  Form1.LargeFileEvent.Set();//lgh
                Form1.WriteFile("strtmpdatccc =" + strtmpdata + "\n");

            Form1.nextsend = true;



            Form1.russiannextEvent.Set();

            //  Form1.LargeFileEvent.Set();//lgh
            //Form1.WriteFile("strtmpdatccc =" + strtmpdata + "\n");

            Form1.nextsend = true;


            Form1.WriteFile("len55=" + AudioData.Length + "\n");
            return;
        }

        public class StringUtil
        {

            /// <summary>
            /// 字节数组转16进制字符串
            /// </summary>
            /// <param name="bytes"></param>
            /// <returns></returns>
            public static string byteToHexStr(byte[] bytes)
            {
                string returnStr = "";
                if (bytes != null)
                {
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        returnStr += bytes[i].ToString("X2");
                    }
                }
                return returnStr;
            }

            /// <summary>
            /// 获取时间戳
            /// </summary>
            /// <returns></returns>
            public static string GetTimeStamp()
            {
                TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
                return Convert.ToInt64(ts.TotalSeconds).ToString();
            }

            /*public static RtaData ParseRtaData(string text)
            {
                try
                {
                    //StreamReader sr1 = new StreamReader("E:\\code\\jsonData.txt", Encoding.Default);
                    //string jsonStr = sr1.ReadToEnd();
                    StringReader sr = new StringReader(text);

                    JsonSerializer serializer = new JsonSerializer();
                    RtaData rta = (RtaData)serializer.Deserialize(new JsonTextReader(sr), typeof(RtaData));
                    return rta;

                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("testJson exception:" + ex.Message);
                }
                return null;
            }*/

            
            

            public static string UrlEncode(string str)
            {
                StringBuilder sb = new StringBuilder();
                byte[] byStr = System.Text.Encoding.UTF8.GetBytes(str); //默认是System.Text.Encoding.Default.GetBytes(str)
                for (int i = 0; i < byStr.Length; i++)
                {
                    sb.Append(@"%" + Convert.ToString(byStr[i], 16));
                }

                return (sb.ToString());
            }
        }

        public static string GetMd5Code(string text)
        {
            try
            {
                //MD5类是抽象类
                MD5 md5 = MD5.Create();
                //需要将字符串转成字节数组
                byte[] buffer = Encoding.Default.GetBytes(text);
                //加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择
                byte[] md5buffer = md5.ComputeHash(buffer);
                string str = null;
                // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
                foreach (byte b in md5buffer)
                {
                    //得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符 
                    //但是在和对方测试过程中，发现我这边的MD5加密编码，经常出现少一位或几位的问题；
                    //后来分析发现是 字符串格式符的问题， X 表示大写， x 表示小写， 
                    //X2和x2表示不省略首位为0的十六进制数字；
                    str += b.ToString("x2");
                }
                Console.WriteLine(str);//202cb962ac59075b964b07152d234b70
                return str;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("GetMd5Code exception:" + e.Message);
            }
            return null;
        }


        /// <summary>
        /// HMACSHA1加密
        /// </summary>
        /// <param name="text">要加密的原串</param>
        ///<param name="key">私钥</param>
        /// <returns></returns>
        static public string HMACSHA1Text(string text, string key)
        {


            try
            {
                //HMACSHA1加密
                HMACSHA1 hmacsha1 = new HMACSHA1
                {
                    Key = System.Text.Encoding.UTF8.GetBytes(key)
                };

                byte[] dataBuffer = System.Text.Encoding.UTF8.GetBytes(text);
                byte[] hashBytes = hmacsha1.ComputeHash(dataBuffer);

                return Convert.ToBase64String(hashBytes);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("HMACSHA1Text exception:" + e.Message);
            }

            return null;
        }
        private static string wsUrl = "ws://rtasr.xfyun.cn/v1/ws";
        /*async public static void Tasker(byte[] AudioData, Form1 tmpform)
        {
            string timeStamp = StringUtil.GetTimeStamp();
            string md5Code = GetMd5Code(x_appid + timeStamp);
            string hmacsha1Code = HMACSHA1Text(md5Code, api_key);

            hmacsha1Code = StringUtil.UrlEncode(hmacsha1Code);
            webSocket0 = new ClientWebSocket();
            try
            {
                await webSocket0.ConnectAsync(new Uri(wsUrl + "?appid=" + x_appid + "&ts=" + timeStamp + "&signa=" + hmacsha1Code), cancellation);



                //连接成功，开始发送数据
                int frameSize = 122 * 8; //每一帧音频的大小,建议每 40ms 发送 122B
                int intervel = 10;
                int status = 0;  // 音频的状态
                while (true)
                {
                    //AudioData
                      // var AudioData = File.ReadAllBytes("test_1.pcm");
                    byte[] buffer ;
                    for (int i = 0; i < AudioData.Length; i += frameSize)
                    {
                        buffer = SubArray(AudioData, i, frameSize);
                        await Task.Delay(intervel); //模拟音频采样延时
                                                    //var frameData = System.Text.Encoding.UTF8.GetBytes(buffer);
                        webSocket0.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, cancellation);
                        await Task.Delay(intervel); //模拟音频采样延时
                    }

                    Thread.Sleep(1000);
                }




            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return ;

            //var AudioData = File.ReadAllBytes(path);
            //Form1.SetKeep();
            //Console.WriteLine();
            Form1.WriteFile("len11=" + AudioData.Length + "\n");
            tmpformdata = tmpform;
            Process p = new Process();
            // p.StartInfo.FileName = @"E:\Program Files (x86)\python\python.exe";//没有配环境变量的话，可以像我这样写python.exe的绝对路径。如果配了，直接写"python.exe"即可
            p.StartInfo.FileName = presure;//没有配环境变量的话，可以像我这样写python.exe的绝对路径。如果配了，直接写"python.exe"即可
            //p.StartInfo.FileName = @"C:\Users\Administrator\AppData\Local\Programs\Python\Python312-32\python3.exe";//没有配环境变量的话，可以像我这样写python.exe的绝对路径。如果配了，直接写"python.exe"即可
            p.StartInfo.Arguments = "rtasr_python3_demo.py";
            //p.StartInfo.Arguments = "iat_ws_python3.py zh_ch";

            p.StartInfo.UseShellExecute = false;

            p.StartInfo.RedirectStandardOutput = true;

            p.StartInfo.RedirectStandardInput = true;

            p.StartInfo.RedirectStandardError = true;

            p.StartInfo.CreateNoWindow = true;

            p.Start();
            Form1.WriteFile("len22=" + AudioData.Length + "\n");
            p.BeginOutputReadLine();
            Form1.WriteFile("len33=" + AudioData.Length + "\n");
            p.OutputDataReceived += new DataReceivedEventHandler(p_OutputChineseDataReceived);
            Form1.WriteFile("len44=" + AudioData.Length + "\n");
            //Console.ReadLine();
            p.WaitForExit();


            lastdata += chineseres;
            //string strtmpdata = tmpform.originaltext.Text;


            Form1.WriteFile("strtmpdataaa =" + lastdata + "\n");

            //string totalstr = strtmpdata + resultObj.GetResultText();
            string resultB = lastdata;
            resultB = resultB.Replace("，", "\n");
            //this.originaltext.Text = resultB;
            tmpform.originaltext.Text = resultB;//lgh

            if (chineseres != "")
            {
                Form1.CLargeFileInfo fileinfo = new Form1.CLargeFileInfo();

                fileinfo.currentdata.AddRange(AudioData);
                fileinfo.translatedata.AddRange(System.Text.Encoding.Default.GetBytes(lastdata));

                //Form1.listlargedata.Add(fileinfo);
                //Form1.WriteFile("will add total  totalstr=" + totalstr + "\n");
                Form1.translatelistmutex.WaitOne();
                Form1.translatelisttdata.AddRange(System.Text.Encoding.Default.GetBytes(lastdata));
                Form1.translatelistmutex.ReleaseMutex();
                Form1.translateEvent.Set();//lgh
            }

            Form1.WriteFile("strtmpdatbbb =" + lastdata + "\n");

            Form1.chinesenextEvent.Set();

            //  Form1.LargeFileEvent.Set();//lgh
            Form1.WriteFile("strtmpdatccc =" + lastdata + "\n");

            Form1.nextsend = true;

            return;

            Form1.WriteFile("len11=" + AudioData.Length + "\n");
           // tmpform.originaltext.Text = "11";
            // 构建鉴权url
            string authUrl = GetAuthUrl();
            string url = authUrl.Replace("http://", "ws://").Replace("https://", "wss://");
            using (webSocket0 = new ClientWebSocket())
            {
                try
                {
                    await webSocket0.ConnectAsync(new Uri(url), cancellation);
                    
                    byte[] ReceiveBuff = new byte[102400];//根据实际情况设置大小
                    var receive = webSocket0.ReceiveAsync(new ArraySegment<byte>(ReceiveBuff), cancellation);
                    
                    //连接成功，开始发送数据
                    int frameSize = 122 * 8; //每一帧音频的大小,建议每 40ms 发送 122B
                    int intervel = 10;
                    int status = 0;  // 音频的状态

                    byte[] buffer ;
                    // 发送音频
                    for (int i = 0; i < AudioData.Length; i += frameSize)
                    {
                        buffer = SubArray(AudioData, i, frameSize);
                        if (buffer == null)
                        {
                            status = StatusLastFrame;  //文件读完，改变status 为 2
                        }
                        switch (status)
                        {
                            case StatusFirstFrame:   // 第一帧音频status = 0
                                JObject frame = new JObject();
                                JObject business = new JObject();  //第一帧必须发送
                                JObject common = new JObject();  //第一帧必须发送
                                JObject data = new JObject();  //每一帧都要发送                            
                                                               // 填充common
                                Form1.WriteFile("len 11=" + AudioData.Length + "\n");
                                common.Add("app_id", x_appid);
                                //填充business                              
                                business.Add("language", "zh_cn");
                                business.Add("domain", "iat");
                                business.Add("accent", "mandarin");
                                //business.Add("nunum", 0);
                                //business.Add("ptt", 0);//标点符号
                                //business.Add("rlang", "zh-hk"); // zh-cn :简体中文（默认值）zh-hk :繁体香港(若未授权不生效)
                                //business.Add("vinfo", 1);
                                //business.Add("dwa", "wpgs");//动态修正(若未授权不生效)
                                //business.Add("nbest", 5);// 句子多候选(若未授权不生效)
                                //business.Add("wbest", 3);// 词级多候选(若未授权不生效)
                                //填充data
                                data.Add("status", StatusFirstFrame);
                                data.Add("format", "audio/L16;rate=16000");
                                data.Add("audio", Convert.ToBase64String(buffer));
                                data.Add("encoding", "raw");
                                //填充frame
                                frame.Add("common", common);
                                frame.Add("business", business);
                                frame.Add("data", data);

                                var frameData = System.Text.Encoding.UTF8.GetBytes(frame.ToString());
                                webSocket0.SendAsync(new ArraySegment<byte>(frameData), WebSocketMessageType.Text, true, cancellation);
                                Console.WriteLine("cc");
                                //webSocket.Send(JsonUtility.ToJson(frame));
                                status = StatusContinueFrame;  // 发送完第一帧改变status 为 1
                                break;
                            case StatusContinueFrame:  //中间帧status = 1
                                JObject frame1 = new JObject();
                                JObject data1 = new JObject();  //每一帧都要发送                                                                                                                        
                                //填充data
                                data1.Add("status", StatusContinueFrame);
                                data1.Add("format", "audio/L16;rate=16000");
                                data1.Add("audio", Convert.ToBase64String(buffer));
                                data1.Add("encoding", "raw");
                                //填充frame
                                frame1.Add("data", data1);
                                //TextFile = File.Open(@".\translationlog.txt", FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                                //datainfo = System.Text.Encoding.Default.GetBytes("len22=" + AudioData.Length + "\n");
                                //Form1.WriteFile("len 22" + AudioData.Length + "\n");
                                //TextFile.Write(datainfo, 0, datainfo.Length);
                                //TextFile.Close();
                                var frameData1 = System.Text.Encoding.UTF8.GetBytes(frame1.ToString());
                                webSocket0.SendAsync(new ArraySegment<byte>(frameData1), WebSocketMessageType.Text, true, cancellation);
                                //Console.WriteLine("dd");
                                //webSocket.Send(JsonUtility.ToJson(frame1));
                                break;
                            case StatusLastFrame:    // 最后一帧音频status = 2 ，标志音频发送结束    
                                break;
                        }
                        await Task.Delay(intervel); //模拟音频采样延时
                    }

                    
                    #region 结束
                    // Console.WriteLine("准备发送最后一段");
                    JObject frame2 = new JObject();
                    JObject data2 = new JObject();  //每一帧都要发送                                                                                                                        
                                                    //填充data
                    data2.Add("status", StatusLastFrame);
                    //填充frame
                    frame2.Add("data", data2);

                    var frameData2 = System.Text.Encoding.UTF8.GetBytes(frame2.ToString());
                    webSocket0.SendAsync(new ArraySegment<byte>(frameData2), WebSocketMessageType.Text, true, cancellation);
                    // Console.WriteLine("发送最后一段结束");

                    //int intervel1 = 1000;
                    await Task.Delay(intervel);

                    #endregion
                    
                    Form1.WriteFile("len 33=" + AudioData.Length + "\n");
                    await receive;
                    
                    int reLength = receive.Result.Count;
                    //Form1.WriteFile("len reLength=" + reLength + "\n");
                    var reData = SubArray(ReceiveBuff, 0, reLength);
                    //Form1.WriteFile("len 332=" + reData + "\n");
                    var ReceviceStr = System.Text.Encoding.UTF8.GetString(reData);
                    
                    //Form1.WriteFile("ReceviceStr=" + ReceviceStr + "\n");
                    var resultObj = GetResultData(ReceviceStr);
                    
                    byte[] byteArray = System.Text.Encoding.Default.GetBytes(resultObj.GetResultText());
                    //lgh
                    lastdata += resultObj.GetResultText();
                    //string strtmpdata = tmpform.originaltext.Text;

                    
                    Form1.WriteFile("strtmpdataaa =" + lastdata + "\n");
                    
                    //string totalstr = strtmpdata + resultObj.GetResultText();
                    resultB = lastdata;
                    resultB = resultB.Replace("，", "\n");
                    //this.originaltext.Text = resultB;
                    tmpform.originaltext.Text = resultB;//lgh
                    
                    //Form1.listlargedata.currentdata.AddRange(System.Text.Encoding.Default.GetBytes(totalstr));
                    if (resultObj.GetResultText().Length > 0)
                    {
                        Form1.CLargeFileInfo fileinfo = new Form1.CLargeFileInfo();
                        
                        fileinfo.currentdata.AddRange(AudioData);
                        fileinfo.translatedata.AddRange(System.Text.Encoding.Default.GetBytes(lastdata));

                        //Form1.listlargedata.Add(fileinfo);
                        //Form1.WriteFile("will add total  totalstr=" + totalstr + "\n");
                       Form1.translatelistmutex.WaitOne();
                        Form1.translatelisttdata.AddRange(System.Text.Encoding.Default.GetBytes(lastdata));
                        Form1.translatelistmutex.ReleaseMutex();
                        Form1.translateEvent.Set();//lgh
                    }

                    Form1.WriteFile("strtmpdatbbb =" + lastdata + "\n");
                    
                    Form1.chinesenextEvent.Set();

                  //  Form1.LargeFileEvent.Set();//lgh
                    Form1.WriteFile("strtmpdatccc =" + lastdata + "\n");
                   
                    Form1.nextsend = true;
                   
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }*/

        async public static void EnglishTasker(byte[] AudioData, Form1 tmpform)
        {
           
            Form1.WriteFile("len11=" + AudioData.Length + "\n");

            Form1.WriteFile("len11=" + AudioData.Length + "\n");

            Process p = new Process();
            // p.StartInfo.FileName = @"E:\Program Files (x86)\python\python.exe";//没有配环境变量的话，可以像我这样写python.exe的绝对路径。如果配了，直接写"python.exe"即可
            p.StartInfo.FileName = presure;//没有配环境变量的话，可以像我这样写python.exe的绝对路径。如果配了，直接写"python.exe"即可
            //p.StartInfo.FileName = @"C:\Users\Administrator\AppData\Local\Programs\Python\Python312-32\python3.exe";//没有配环境变量的话，可以像我这样写python.exe的绝对路径。如果配了，直接写"python.exe"即可
            p.StartInfo.Arguments = "iat_ws_python3.py en_us";

            p.StartInfo.UseShellExecute = false;

            p.StartInfo.RedirectStandardOutput = true;

            p.StartInfo.RedirectStandardInput = true;

            p.StartInfo.RedirectStandardError = true;

            p.StartInfo.CreateNoWindow = true;

            p.Start();
            Form1.WriteFile("len22=" + AudioData.Length + "\n");
            p.BeginOutputReadLine();
            Form1.WriteFile("len33=" + AudioData.Length + "\n");
            p.OutputDataReceived += new DataReceivedEventHandler(p_OutputChineseDataReceived);
            Form1.WriteFile("len44=" + AudioData.Length + "\n");
            //Console.ReadLine();
            p.WaitForExit();


            lastdata += chineseres;
            //string strtmpdata = tmpform.originaltext.Text;


            Form1.WriteFile("strtmpdataaa =" + lastdata + "\n");

            //string totalstr = strtmpdata + resultObj.GetResultText();
            string resultB = lastdata;
            resultB = resultB.Replace("，", "\n");
            //this.originaltext.Text = resultB;
            tmpform.originaltext.Text = resultB;//lgh

            if (chineseres != "")
            {
                Form1.CLargeFileInfo fileinfo = new Form1.CLargeFileInfo();

                //fileinfo.currentdata.AddRange(AudioData);
                //fileinfo.translatedata.AddRange(System.Text.Encoding.Default.GetBytes(lastdata));

                //Form1.listlargedata.Add(fileinfo);
                //Form1.WriteFile("will add total  totalstr=" + totalstr + "\n");
                Form1.englishtranslatelistmutex.WaitOne();
                Form1.englishtranslatelisttdata.AddRange(System.Text.Encoding.Default.GetBytes(lastdata));
                Form1.englishtranslatelistmutex.ReleaseMutex();
                Form1.englishtranslateEvent.Set();//lgh
            }

            Form1.WriteFile("strtmpdatbbb =" + lastdata + "\n");

            Form1.englishnextEvent.Set();

            //  Form1.LargeFileEvent.Set();//lgh
            Form1.WriteFile("strtmpdatccc =" + lastdata + "\n");

            Form1.nextsend = true;

            return;

            // tmpform.originaltext.Text = "11";
            // 构建鉴权url
            string authUrl = GetAuthUrl();
            string url = authUrl.Replace("http://", "ws://").Replace("https://", "wss://");
            using (webSocket0 = new ClientWebSocket())
            {
                try
                {
                    await webSocket0.ConnectAsync(new Uri(url), cancellation);

                    byte[] ReceiveBuff = new byte[102400];//根据实际情况设置大小
                    var receive = webSocket0.ReceiveAsync(new ArraySegment<byte>(ReceiveBuff), cancellation);

                    //连接成功，开始发送数据
                    int frameSize = 122 * 8; //每一帧音频的大小,建议每 40ms 发送 122B
                    int intervel = 10;
                    int status = 0;  // 音频的状态

                    byte[] buffer /*= new byte[frameSize]*/;
                    // 发送音频
                    for (int i = 0; i < AudioData.Length; i += frameSize)
                    {
                        buffer = SubArray(AudioData, i, frameSize);
                        if (buffer == null)
                        {
                            status = StatusLastFrame;  //文件读完，改变status 为 2
                        }
                        switch (status)
                        {
                            case StatusFirstFrame:   // 第一帧音频status = 0
                                JObject frame = new JObject();
                                JObject business = new JObject();  //第一帧必须发送
                                JObject common = new JObject();  //第一帧必须发送
                                JObject data = new JObject();  //每一帧都要发送                            
                                                               // 填充common
                                Form1.WriteFile("len 11=" + AudioData.Length + "\n");
                                //common.Add("app_id", Form1.x_appid);
                                //填充business                              
                                business.Add("language", "en_us");
                                business.Add("domain", "iat");
                                business.Add("accent", "mandarin");
                                //填充data
                                data.Add("status", StatusFirstFrame);
                                data.Add("format", "audio/L16;rate=16000");
                                data.Add("audio", Convert.ToBase64String(buffer));
                                data.Add("encoding", "raw");
                                //填充frame
                                frame.Add("common", common);
                                frame.Add("business", business);
                                frame.Add("data", data);

                                var frameData = System.Text.Encoding.UTF8.GetBytes(frame.ToString());
                                webSocket0.SendAsync(new ArraySegment<byte>(frameData), WebSocketMessageType.Text, true, cancellation);
                                Console.WriteLine("cc");
                                //webSocket.Send(JsonUtility.ToJson(frame));
                                status = StatusContinueFrame;  // 发送完第一帧改变status 为 1
                                break;
                            case StatusContinueFrame:  //中间帧status = 1
                                JObject frame1 = new JObject();
                                JObject data1 = new JObject();  //每一帧都要发送                                                                                                                        
                                //填充data
                                data1.Add("status", StatusContinueFrame);
                                data1.Add("format", "audio/L16;rate=16000");
                                data1.Add("audio", Convert.ToBase64String(buffer));
                                data1.Add("encoding", "raw");
                                //填充frame
                                frame1.Add("data", data1);
                                //TextFile = File.Open(@".\translationlog.txt", FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                                //datainfo = System.Text.Encoding.Default.GetBytes("len22=" + AudioData.Length + "\n");
                                //Form1.WriteFile("len 22" + AudioData.Length + "\n");
                                //TextFile.Write(datainfo, 0, datainfo.Length);
                                //TextFile.Close();
                                var frameData1 = System.Text.Encoding.UTF8.GetBytes(frame1.ToString());
                                webSocket0.SendAsync(new ArraySegment<byte>(frameData1), WebSocketMessageType.Text, true, cancellation);
                                //Console.WriteLine("dd");
                                //webSocket.Send(JsonUtility.ToJson(frame1));
                                break;
                            case StatusLastFrame:    // 最后一帧音频status = 2 ，标志音频发送结束    
                                break;
                        }
                        await Task.Delay(intervel); //模拟音频采样延时
                    }


                    #region 结束
                    // Console.WriteLine("准备发送最后一段");
                    JObject frame2 = new JObject();
                    JObject data2 = new JObject();  //每一帧都要发送                                                                                                                        
                                                    //填充data
                    data2.Add("status", StatusLastFrame);
                    //填充frame
                    frame2.Add("data", data2);

                    var frameData2 = System.Text.Encoding.UTF8.GetBytes(frame2.ToString());
                    webSocket0.SendAsync(new ArraySegment<byte>(frameData2), WebSocketMessageType.Text, true, cancellation);

                    await Task.Delay(intervel);

                    #endregion

                    Form1.WriteFile("len 33=" + AudioData.Length + "\n");
                    await receive;

                    int reLength = receive.Result.Count;
                    //Form1.WriteFile("len reLength=" + reLength + "\n");
                    var reData = SubArray(ReceiveBuff, 0, reLength);
                    //Form1.WriteFile("len 332=" + reData + "\n");
                    var ReceviceStr = System.Text.Encoding.UTF8.GetString(reData);

                    //Form1.WriteFile("ReceviceStr=" + ReceviceStr + "\n");
                    var resultObj = GetResultData(ReceviceStr);

                    byte[] byteArray = System.Text.Encoding.Default.GetBytes(resultObj.GetResultText());
                    //lgh
                    string strtmpdata = tmpform.originaltext.Text;


                    Form1.WriteFile("strtmpdataaa =" + strtmpdata + "\n");

                    string totalstr = strtmpdata + resultObj.GetResultText();
                    tmpform.originaltext.Text = totalstr;//lgh

                    //Form1.listlargedata.currentdata.AddRange(System.Text.Encoding.Default.GetBytes(totalstr));
                    if (totalstr.Length > 0)
                    {
                        Form1.CLargeFileInfo fileinfo = new Form1.CLargeFileInfo();

                        fileinfo.currentdata.AddRange(AudioData);
                        fileinfo.translatedata.AddRange(System.Text.Encoding.Default.GetBytes(totalstr));

                        //Form1.listlargedata.Add(fileinfo);
                        //Form1.WriteFile("will add total  totalstr=" + totalstr + "\n");
                        Form1.englishtranslatelistmutex.WaitOne();
                        Form1.englishtranslatelisttdata.AddRange(System.Text.Encoding.Default.GetBytes(totalstr));
                        Form1.englishtranslatelistmutex.ReleaseMutex();
                        Form1.englishtranslateEvent.Set();//lgh
                    }

                    Form1.WriteFile("strtmpdatbbb =" + strtmpdata + "\n");

                    Form1.englishnextEvent.Set();

                    //  Form1.LargeFileEvent.Set();//lgh
                    Form1.WriteFile("strtmpdatccc =" + strtmpdata + "\n");

                    Form1.nextsend = true;

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        async public static void LargeTasker(byte[] AudioData)
        {
            //var AudioData = File.ReadAllBytes(path);
            //Form1.SetKeep();
            //Console.WriteLine();

            Form1.WriteFile("len11=" + AudioData.Length + "\n");

            // 构建鉴权url
            string authUrl = GetAuthUrl();
            string url = authUrl.Replace("http://", "ws://").Replace("https://", "wss://");
            using (webSocket0 = new ClientWebSocket())
            {
                try
                {
                    await webSocket0.ConnectAsync(new Uri(url), cancellation);
                    //Console.WriteLine("aa");
                    byte[] ReceiveBuff = new byte[102400];//根据实际情况设置大小
                    var receive = webSocket0.ReceiveAsync(new ArraySegment<byte>(ReceiveBuff), cancellation);
                    //Console.WriteLine("bb");
                    //连接成功，开始发送数据
                    int frameSize = 122 * 8; //每一帧音频的大小,建议每 40ms 发送 122B
                    int intervel = 10;
                    int status = 0;  // 音频的状态

                    byte[] buffer /*= new byte[frameSize]*/;
                    // 发送音频
                    for (int i = 0; i < AudioData.Length; i += frameSize)
                    {
                        buffer = SubArray(AudioData, i, frameSize);
                        if (buffer == null)
                        {
                            status = StatusLastFrame;  //文件读完，改变status 为 2
                        }
                        switch (status)
                        {
                            case StatusFirstFrame:   // 第一帧音频status = 0
                                JObject frame = new JObject();
                                JObject business = new JObject();  //第一帧必须发送
                                JObject common = new JObject();  //第一帧必须发送
                                JObject data = new JObject();  //每一帧都要发送                            
                                                               // 填充common
                                Form1.WriteFile("len 11=" + AudioData.Length + "\n");
                                //common.Add("app_id", x_appid);
                                //填充business                              
                                business.Add("language", "zh_cn");
                                business.Add("domain", "iat");
                                business.Add("accent", "mandarin");
                                //business.Add("nunum", 0);
                                //business.Add("ptt", 0);//标点符号
                                //business.Add("rlang", "zh-hk"); // zh-cn :简体中文（默认值）zh-hk :繁体香港(若未授权不生效)
                                //business.Add("vinfo", 1);
                                //business.Add("dwa", "wpgs");//动态修正(若未授权不生效)
                                //business.Add("nbest", 5);// 句子多候选(若未授权不生效)
                                //business.Add("wbest", 3);// 词级多候选(若未授权不生效)
                                //填充data
                                data.Add("status", StatusFirstFrame);
                                data.Add("format", "audio/L16;rate=16000");
                                data.Add("audio", Convert.ToBase64String(buffer));
                                data.Add("encoding", "raw");
                                //填充frame
                                frame.Add("common", common);
                                frame.Add("business", business);
                                frame.Add("data", data);

                                //Console.WriteLine(frame.ToString());
                                var frameData = System.Text.Encoding.UTF8.GetBytes(frame.ToString());
                                webSocket0.SendAsync(new ArraySegment<byte>(frameData), WebSocketMessageType.Text, true, cancellation);
                                Console.WriteLine("cc");
                                //webSocket.Send(JsonUtility.ToJson(frame));
                                status = StatusContinueFrame;  // 发送完第一帧改变status 为 1
                                break;
                            case StatusContinueFrame:  //中间帧status = 1
                                JObject frame1 = new JObject();
                                JObject data1 = new JObject();  //每一帧都要发送                                                                                                                        
                                //填充data
                                data1.Add("status", StatusContinueFrame);
                                data1.Add("format", "audio/L16;rate=16000");
                                data1.Add("audio", Convert.ToBase64String(buffer));
                                data1.Add("encoding", "raw");
                                //填充frame
                                frame1.Add("data", data1);
                                //TextFile = File.Open(@".\translationlog.txt", FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                                //datainfo = System.Text.Encoding.Default.GetBytes("len22=" + AudioData.Length + "\n");
                                //Form1.WriteFile("len 22" + AudioData.Length + "\n");
                                //TextFile.Write(datainfo, 0, datainfo.Length);
                                //TextFile.Close();
                                var frameData1 = System.Text.Encoding.UTF8.GetBytes(frame1.ToString());
                                webSocket0.SendAsync(new ArraySegment<byte>(frameData1), WebSocketMessageType.Text, true, cancellation);
                                //Console.WriteLine("dd");
                                //webSocket.Send(JsonUtility.ToJson(frame1));
                                break;
                            case StatusLastFrame:    // 最后一帧音频status = 2 ，标志音频发送结束    
                                break;
                        }
                        await Task.Delay(intervel); //模拟音频采样延时
                    }

                    #region 结束
                    // Console.WriteLine("准备发送最后一段");
                    JObject frame2 = new JObject();
                    JObject data2 = new JObject();  //每一帧都要发送                                                                                                                        
                                                    //填充data
                    data2.Add("status", StatusLastFrame);
                    //填充frame
                    frame2.Add("data", data2);

                    var frameData2 = System.Text.Encoding.UTF8.GetBytes(frame2.ToString());
                    webSocket0.SendAsync(new ArraySegment<byte>(frameData2), WebSocketMessageType.Text, true, cancellation);
                    // Console.WriteLine("发送最后一段结束");

                    //int intervel1 = 1000;
                    await Task.Delay(intervel);

                    #endregion
                    Form1.WriteFile("len 33=" + AudioData.Length + "\n");
                    await receive;

                    int reLength = receive.Result.Count;
                    Form1.WriteFile("len reLength=" + reLength + "\n");
                    var reData = SubArray(ReceiveBuff, 0, reLength);
                    Form1.WriteFile("len 332=" + reData + "\n");
                    var ReceviceStr = System.Text.Encoding.UTF8.GetString(reData);
                    //Console.WriteLine(ReceviceStr);
                    Form1.WriteFile("ReceviceStr=" + ReceviceStr + "\n");
                    var resultObj = GetResultData(ReceviceStr);
                    //Form1.WriteFile("len 333=" + AudioData.Length + "\n");
                    byte[] byteArray = System.Text.Encoding.Default.GetBytes(resultObj.GetResultText());
                    //Form1.WriteFile("len 334=" + AudioData.Length + "\n");
                   /* string strtmpdata = Form1.originaltext.Text;


                    Form1.WriteFile("strtmpdataaa =" + strtmpdata + "\n");

                    string totalstr = strtmpdata + resultObj.GetResultText();
                    Form1.originaltext.Text = totalstr;
                    if (totalstr.Length > 0)
                    {

                        Form1.translatelistmutex.WaitOne();
                        Form1.translatelisttdata.AddRange(System.Text.Encoding.Default.GetBytes(totalstr));
                        Form1.translatelistmutex.ReleaseMutex();
                        Form1.translateEvent.Set();
                    }

                    // datainfo = System.Text.Encoding.Default.GetBytes("strtmpdata111 =" + strtmpdata + "\n");

                    Form1.WriteFile("strtmpdatbbb =" + strtmpdata + "\n");

                    //datainfo = System.Text.Encoding.Default.GetBytes("strtmpdata222 =" + strtmpdata + "\n");
                    Form1.largefilenextEvent.Set();
                    Form1.WriteFile("strtmpdatccc =" + strtmpdata + "\n");

                    //Console.WriteLine(resultObj.GetResultText());
                    Form1.nextsend = true;*/
                    //datainfo = System.Text.Encoding.Default.GetBytes("strtmpdata333 =" + strtmpdata + "\n");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        // 返回code为错误码时，请查询https://www.xfyun.cn/document/error-code解决方案
        static string GetAuthUrl()
        {
            /*    string date = DateTime.UtcNow.ToString("r");

                Uri uri = new Uri(hostUrl);
                StringBuilder builder = new StringBuilder("host: ").Append(uri.Host).Append("\n").//
                                        Append("date: ").Append(date).Append("\n").//
                                        Append("GET ").Append(uri.LocalPath).Append(" HTTP/1.1");

                string sha = HMACsha256(api_secret, builder.ToString());
                string authorization = string.Format("api_key=\"{0}\", algorithm=\"{1}\", headers=\"{2}\", signature=\"{3}\"", api_key, "hmac-sha256", "host date request-line", sha);
                //System.Web.HttpUtility.UrlEncode

                string NewUrl = "https://" + uri.Host + uri.LocalPath;

                string path1 = "authorization" + "=" + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(authorization));
                date = date.Replace(" ", "%20").Replace(":", "%3A").Replace(",", "%2C");
                string path2 = "date" + "=" + date;
                string path3 = "host" + "=" + uri.Host;

                NewUrl = NewUrl + "?" + path1 + "&" + path2 + "&" + path3;
                return NewUrl;*/
            return "";
        }

        public static string HMACsha256(string apiSecretIsKey, string buider)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(apiSecretIsKey);
            System.Security.Cryptography.HMACSHA256 hMACSHA256 = new System.Security.Cryptography.HMACSHA256(bytes);
            byte[] date = System.Text.Encoding.UTF8.GetBytes(buider);
            date = hMACSHA256.ComputeHash(date);
            hMACSHA256.Clear();

            return Convert.ToBase64String(date);

        }
        /// <summary>
        /// 从此实例检索子数组
        /// </summary>
        /// <param name="source">要检索的数组</param>
        /// <param name="startIndex">起始索引号</param>
        /// <param name="length">检索最大长度</param>
        /// <returns>与此实例中在 startIndex 处开头、长度为 length 的子数组等效的一个数组</returns>
        public static byte[] SubArray(byte[] source, int startIndex, int length)
        {

            if (startIndex < 0 || startIndex > source.Length || length < 0)
            {
                return null;
            }
            byte[] Destination;
            if (startIndex + length <= source.Length)
            {
                Destination = new byte[length];
                Array.Copy(source, startIndex, Destination, 0, length);
            }
            else
            {
                Destination = new byte[(source.Length - startIndex)];
                Array.Copy(source, startIndex, Destination, 0, source.Length - startIndex);
            }

            return Destination;
        }
        static private ResponseData GetResultData(string ReceviceStr)
        {
            ResponseData temp = new ResponseData();
            ReaponseDataInfo dataInfo = new ReaponseDataInfo();
            ResponseResultInfo resultInfo = new ResponseResultInfo();
            List<Ws> tempwsS;
            List<Cw> tempcwS;

            Ws tempWs;
            Cw temocw;
            var jsonObj = (JObject)JsonConvert.DeserializeObject(ReceviceStr);
            //Debug.Log("1");
            temp.code = jsonObj["code"].ToObject<int>();
            temp.message = jsonObj["message"].ToObject<string>();
            temp.sid = jsonObj["sid"].ToObject<string>();
            Form1.WriteFile("sid=" + temp.sid + "\n");
            var data = jsonObj["data"]/*.ToObject<JObject>()*/;
            //Debug.Log("2");
            dataInfo.status = data["status"].ToObject<int>();
            var result = data["result"]/*.ToObject<JObject>()*/;
            //Debug.Log("3");
            resultInfo.bg = result["bg"].ToObject<int>();
            resultInfo.ed = result["ed"].ToObject<int>();
            //resultInfo.pgs = result["pgs"].ToObject<string>();
            //resultInfo.rg = result["rg"].ToObject<int[]>(); 
            resultInfo.sn = result["sn"].ToObject<int>(); ;
            resultInfo.ls = result["ls"].ToObject<bool>(); ;
            var wss = result["ws"];
            //Debug.Log("4");
            tempwsS = new List<Ws>();
            JArray wsArray = wss.ToObject<JArray>();
            //Debug.Log("5.0");
            for (int i = 0; i < wsArray.Count; i++)
            {
                //Debug.Log("5.1");
                tempWs = new Ws();
                tempWs.bg = wsArray[i]["bg"].ToObject<int>();
                //Debug.Log("5.2");
                //tempWs.ed = wsArray[i]["ed"].ToObject<int>();
                var cws = wsArray[i]["cw"];
                //Debug.Log("5.5");
                tempcwS = new List<Cw>();
                JArray cwArray = cws.ToObject<JArray>();
                for (int j = 0; j < cwArray.Count; j++)
                {
                    temocw = new Cw();
                    temocw.sc = cwArray[j]["sc"].ToObject<int>();
                    temocw.w = cwArray[j]["w"].ToObject<string>();
                    tempcwS.Add(temocw);
                }
                tempWs.cw = tempcwS.ToArray();
                tempwsS.Add(tempWs);
            }
            //Debug.Log("6");
            resultInfo.ws = tempwsS.ToArray();
            dataInfo.result = resultInfo;
            temp.data = dataInfo;
            return temp;
            //int cod = jobj1.ToObject<int>();
        }
    }
}