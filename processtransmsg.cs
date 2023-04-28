using System;

using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;  //����Ҫ��Ӹ�����

using System.IO;
using System.Text;
using System.Threading;  //���������ռ�,��Thread���ڴ˿ռ���
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
        // Ӧ��APPID������Ϊwebapi����Ӧ�ã�����ͨ������д���񣬲ο�������δ���һ��webapiӦ�ã�http://bbs.xfyun.cn/forum.php?mod=viewthread&tid=36481��
        
        // ��Ƶ�ļ���ַ,ʾ����Ƶ������д�ӿ��ĵ��ײ�����D
        //static string path = @".//en_sentence.mp3";//�����ļ�·��,�Լ��޸�
        static string chinesepath = @".//chinese.wav";//�����ļ�·��,�Լ��޸�
        static string englishpath = @".//english.wav";//�����ļ�·��,�Լ��޸�
        static string russianpath = @".//russian.wav";//�����ļ�·��,�Լ��޸�
        //static string strdata = @".//test.wav";//�����ļ�·��,�Լ��޸�
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
            // p.StartInfo.FileName = @"E:\Program Files (x86)\python\python.exe";//û���价�������Ļ���������������дpython.exe�ľ���·����������ˣ�ֱ��д"python.exe"����
            //p.StartInfo.FileName = @"C:\Users\Administrator\AppData\Local\Programs\Python\Python312-32\python.exe";//û���价�������Ļ���������������дpython.exe�ľ���·����������ˣ�ֱ��д"python.exe"����
            p.StartInfo.FileName = presure;//û���价�������Ļ���������������дpython.exe�ľ���·����������ˣ�ֱ��д"python.exe"����
            
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
            /// �ֽ�����ת16�����ַ���
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
            /// ��ȡʱ���
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
                byte[] byStr = System.Text.Encoding.UTF8.GetBytes(str); //Ĭ����System.Text.Encoding.Default.GetBytes(str)
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
                //MD5���ǳ�����
                MD5 md5 = MD5.Create();
                //��Ҫ���ַ���ת���ֽ�����
                byte[] buffer = Encoding.Default.GetBytes(text);
                //���ܺ���һ���ֽ����͵����飬����Ҫע�����UTF8/Unicode�ȵ�ѡ��
                byte[] md5buffer = md5.ComputeHash(buffer);
                string str = null;
                // ͨ��ʹ��ѭ�������ֽ����͵�����ת��Ϊ�ַ��������ַ����ǳ����ַ���ʽ������
                foreach (byte b in md5buffer)
                {
                    //�õ����ַ���ʹ��ʮ���������͸�ʽ����ʽ����ַ���Сд����ĸ�����ʹ�ô�д��X�����ʽ����ַ��Ǵ�д�ַ� 
                    //�����ںͶԷ����Թ����У���������ߵ�MD5���ܱ��룬����������һλ��λ�����⣻
                    //�������������� �ַ�����ʽ�������⣬ X ��ʾ��д�� x ��ʾСд�� 
                    //X2��x2��ʾ��ʡ����λΪ0��ʮ���������֣�
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
        /// HMACSHA1����
        /// </summary>
        /// <param name="text">Ҫ���ܵ�ԭ��</param>
        ///<param name="key">˽Կ</param>
        /// <returns></returns>
        static public string HMACSHA1Text(string text, string key)
        {


            try
            {
                //HMACSHA1����
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



                //���ӳɹ�����ʼ��������
                int frameSize = 122 * 8; //ÿһ֡��Ƶ�Ĵ�С,����ÿ 40ms ���� 122B
                int intervel = 10;
                int status = 0;  // ��Ƶ��״̬
                while (true)
                {
                    //AudioData
                      // var AudioData = File.ReadAllBytes("test_1.pcm");
                    byte[] buffer ;
                    for (int i = 0; i < AudioData.Length; i += frameSize)
                    {
                        buffer = SubArray(AudioData, i, frameSize);
                        await Task.Delay(intervel); //ģ����Ƶ������ʱ
                                                    //var frameData = System.Text.Encoding.UTF8.GetBytes(buffer);
                        webSocket0.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, cancellation);
                        await Task.Delay(intervel); //ģ����Ƶ������ʱ
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
            // p.StartInfo.FileName = @"E:\Program Files (x86)\python\python.exe";//û���价�������Ļ���������������дpython.exe�ľ���·����������ˣ�ֱ��д"python.exe"����
            p.StartInfo.FileName = presure;//û���价�������Ļ���������������дpython.exe�ľ���·����������ˣ�ֱ��д"python.exe"����
            //p.StartInfo.FileName = @"C:\Users\Administrator\AppData\Local\Programs\Python\Python312-32\python3.exe";//û���价�������Ļ���������������дpython.exe�ľ���·����������ˣ�ֱ��д"python.exe"����
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
            resultB = resultB.Replace("��", "\n");
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
            // ������Ȩurl
            string authUrl = GetAuthUrl();
            string url = authUrl.Replace("http://", "ws://").Replace("https://", "wss://");
            using (webSocket0 = new ClientWebSocket())
            {
                try
                {
                    await webSocket0.ConnectAsync(new Uri(url), cancellation);
                    
                    byte[] ReceiveBuff = new byte[102400];//����ʵ��������ô�С
                    var receive = webSocket0.ReceiveAsync(new ArraySegment<byte>(ReceiveBuff), cancellation);
                    
                    //���ӳɹ�����ʼ��������
                    int frameSize = 122 * 8; //ÿһ֡��Ƶ�Ĵ�С,����ÿ 40ms ���� 122B
                    int intervel = 10;
                    int status = 0;  // ��Ƶ��״̬

                    byte[] buffer ;
                    // ������Ƶ
                    for (int i = 0; i < AudioData.Length; i += frameSize)
                    {
                        buffer = SubArray(AudioData, i, frameSize);
                        if (buffer == null)
                        {
                            status = StatusLastFrame;  //�ļ����꣬�ı�status Ϊ 2
                        }
                        switch (status)
                        {
                            case StatusFirstFrame:   // ��һ֡��Ƶstatus = 0
                                JObject frame = new JObject();
                                JObject business = new JObject();  //��һ֡���뷢��
                                JObject common = new JObject();  //��һ֡���뷢��
                                JObject data = new JObject();  //ÿһ֡��Ҫ����                            
                                                               // ���common
                                Form1.WriteFile("len 11=" + AudioData.Length + "\n");
                                common.Add("app_id", x_appid);
                                //���business                              
                                business.Add("language", "zh_cn");
                                business.Add("domain", "iat");
                                business.Add("accent", "mandarin");
                                //business.Add("nunum", 0);
                                //business.Add("ptt", 0);//������
                                //business.Add("rlang", "zh-hk"); // zh-cn :�������ģ�Ĭ��ֵ��zh-hk :�������(��δ��Ȩ����Ч)
                                //business.Add("vinfo", 1);
                                //business.Add("dwa", "wpgs");//��̬����(��δ��Ȩ����Ч)
                                //business.Add("nbest", 5);// ���Ӷ��ѡ(��δ��Ȩ����Ч)
                                //business.Add("wbest", 3);// �ʼ����ѡ(��δ��Ȩ����Ч)
                                //���data
                                data.Add("status", StatusFirstFrame);
                                data.Add("format", "audio/L16;rate=16000");
                                data.Add("audio", Convert.ToBase64String(buffer));
                                data.Add("encoding", "raw");
                                //���frame
                                frame.Add("common", common);
                                frame.Add("business", business);
                                frame.Add("data", data);

                                var frameData = System.Text.Encoding.UTF8.GetBytes(frame.ToString());
                                webSocket0.SendAsync(new ArraySegment<byte>(frameData), WebSocketMessageType.Text, true, cancellation);
                                Console.WriteLine("cc");
                                //webSocket.Send(JsonUtility.ToJson(frame));
                                status = StatusContinueFrame;  // �������һ֡�ı�status Ϊ 1
                                break;
                            case StatusContinueFrame:  //�м�֡status = 1
                                JObject frame1 = new JObject();
                                JObject data1 = new JObject();  //ÿһ֡��Ҫ����                                                                                                                        
                                //���data
                                data1.Add("status", StatusContinueFrame);
                                data1.Add("format", "audio/L16;rate=16000");
                                data1.Add("audio", Convert.ToBase64String(buffer));
                                data1.Add("encoding", "raw");
                                //���frame
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
                            case StatusLastFrame:    // ���һ֡��Ƶstatus = 2 ����־��Ƶ���ͽ���    
                                break;
                        }
                        await Task.Delay(intervel); //ģ����Ƶ������ʱ
                    }

                    
                    #region ����
                    // Console.WriteLine("׼���������һ��");
                    JObject frame2 = new JObject();
                    JObject data2 = new JObject();  //ÿһ֡��Ҫ����                                                                                                                        
                                                    //���data
                    data2.Add("status", StatusLastFrame);
                    //���frame
                    frame2.Add("data", data2);

                    var frameData2 = System.Text.Encoding.UTF8.GetBytes(frame2.ToString());
                    webSocket0.SendAsync(new ArraySegment<byte>(frameData2), WebSocketMessageType.Text, true, cancellation);
                    // Console.WriteLine("�������һ�ν���");

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
                    resultB = resultB.Replace("��", "\n");
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
            // p.StartInfo.FileName = @"E:\Program Files (x86)\python\python.exe";//û���价�������Ļ���������������дpython.exe�ľ���·����������ˣ�ֱ��д"python.exe"����
            p.StartInfo.FileName = presure;//û���价�������Ļ���������������дpython.exe�ľ���·����������ˣ�ֱ��д"python.exe"����
            //p.StartInfo.FileName = @"C:\Users\Administrator\AppData\Local\Programs\Python\Python312-32\python3.exe";//û���价�������Ļ���������������дpython.exe�ľ���·����������ˣ�ֱ��д"python.exe"����
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
            resultB = resultB.Replace("��", "\n");
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
            // ������Ȩurl
            string authUrl = GetAuthUrl();
            string url = authUrl.Replace("http://", "ws://").Replace("https://", "wss://");
            using (webSocket0 = new ClientWebSocket())
            {
                try
                {
                    await webSocket0.ConnectAsync(new Uri(url), cancellation);

                    byte[] ReceiveBuff = new byte[102400];//����ʵ��������ô�С
                    var receive = webSocket0.ReceiveAsync(new ArraySegment<byte>(ReceiveBuff), cancellation);

                    //���ӳɹ�����ʼ��������
                    int frameSize = 122 * 8; //ÿһ֡��Ƶ�Ĵ�С,����ÿ 40ms ���� 122B
                    int intervel = 10;
                    int status = 0;  // ��Ƶ��״̬

                    byte[] buffer /*= new byte[frameSize]*/;
                    // ������Ƶ
                    for (int i = 0; i < AudioData.Length; i += frameSize)
                    {
                        buffer = SubArray(AudioData, i, frameSize);
                        if (buffer == null)
                        {
                            status = StatusLastFrame;  //�ļ����꣬�ı�status Ϊ 2
                        }
                        switch (status)
                        {
                            case StatusFirstFrame:   // ��һ֡��Ƶstatus = 0
                                JObject frame = new JObject();
                                JObject business = new JObject();  //��һ֡���뷢��
                                JObject common = new JObject();  //��һ֡���뷢��
                                JObject data = new JObject();  //ÿһ֡��Ҫ����                            
                                                               // ���common
                                Form1.WriteFile("len 11=" + AudioData.Length + "\n");
                                //common.Add("app_id", Form1.x_appid);
                                //���business                              
                                business.Add("language", "en_us");
                                business.Add("domain", "iat");
                                business.Add("accent", "mandarin");
                                //���data
                                data.Add("status", StatusFirstFrame);
                                data.Add("format", "audio/L16;rate=16000");
                                data.Add("audio", Convert.ToBase64String(buffer));
                                data.Add("encoding", "raw");
                                //���frame
                                frame.Add("common", common);
                                frame.Add("business", business);
                                frame.Add("data", data);

                                var frameData = System.Text.Encoding.UTF8.GetBytes(frame.ToString());
                                webSocket0.SendAsync(new ArraySegment<byte>(frameData), WebSocketMessageType.Text, true, cancellation);
                                Console.WriteLine("cc");
                                //webSocket.Send(JsonUtility.ToJson(frame));
                                status = StatusContinueFrame;  // �������һ֡�ı�status Ϊ 1
                                break;
                            case StatusContinueFrame:  //�м�֡status = 1
                                JObject frame1 = new JObject();
                                JObject data1 = new JObject();  //ÿһ֡��Ҫ����                                                                                                                        
                                //���data
                                data1.Add("status", StatusContinueFrame);
                                data1.Add("format", "audio/L16;rate=16000");
                                data1.Add("audio", Convert.ToBase64String(buffer));
                                data1.Add("encoding", "raw");
                                //���frame
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
                            case StatusLastFrame:    // ���һ֡��Ƶstatus = 2 ����־��Ƶ���ͽ���    
                                break;
                        }
                        await Task.Delay(intervel); //ģ����Ƶ������ʱ
                    }


                    #region ����
                    // Console.WriteLine("׼���������һ��");
                    JObject frame2 = new JObject();
                    JObject data2 = new JObject();  //ÿһ֡��Ҫ����                                                                                                                        
                                                    //���data
                    data2.Add("status", StatusLastFrame);
                    //���frame
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

            // ������Ȩurl
            string authUrl = GetAuthUrl();
            string url = authUrl.Replace("http://", "ws://").Replace("https://", "wss://");
            using (webSocket0 = new ClientWebSocket())
            {
                try
                {
                    await webSocket0.ConnectAsync(new Uri(url), cancellation);
                    //Console.WriteLine("aa");
                    byte[] ReceiveBuff = new byte[102400];//����ʵ��������ô�С
                    var receive = webSocket0.ReceiveAsync(new ArraySegment<byte>(ReceiveBuff), cancellation);
                    //Console.WriteLine("bb");
                    //���ӳɹ�����ʼ��������
                    int frameSize = 122 * 8; //ÿһ֡��Ƶ�Ĵ�С,����ÿ 40ms ���� 122B
                    int intervel = 10;
                    int status = 0;  // ��Ƶ��״̬

                    byte[] buffer /*= new byte[frameSize]*/;
                    // ������Ƶ
                    for (int i = 0; i < AudioData.Length; i += frameSize)
                    {
                        buffer = SubArray(AudioData, i, frameSize);
                        if (buffer == null)
                        {
                            status = StatusLastFrame;  //�ļ����꣬�ı�status Ϊ 2
                        }
                        switch (status)
                        {
                            case StatusFirstFrame:   // ��һ֡��Ƶstatus = 0
                                JObject frame = new JObject();
                                JObject business = new JObject();  //��һ֡���뷢��
                                JObject common = new JObject();  //��һ֡���뷢��
                                JObject data = new JObject();  //ÿһ֡��Ҫ����                            
                                                               // ���common
                                Form1.WriteFile("len 11=" + AudioData.Length + "\n");
                                //common.Add("app_id", x_appid);
                                //���business                              
                                business.Add("language", "zh_cn");
                                business.Add("domain", "iat");
                                business.Add("accent", "mandarin");
                                //business.Add("nunum", 0);
                                //business.Add("ptt", 0);//������
                                //business.Add("rlang", "zh-hk"); // zh-cn :�������ģ�Ĭ��ֵ��zh-hk :�������(��δ��Ȩ����Ч)
                                //business.Add("vinfo", 1);
                                //business.Add("dwa", "wpgs");//��̬����(��δ��Ȩ����Ч)
                                //business.Add("nbest", 5);// ���Ӷ��ѡ(��δ��Ȩ����Ч)
                                //business.Add("wbest", 3);// �ʼ����ѡ(��δ��Ȩ����Ч)
                                //���data
                                data.Add("status", StatusFirstFrame);
                                data.Add("format", "audio/L16;rate=16000");
                                data.Add("audio", Convert.ToBase64String(buffer));
                                data.Add("encoding", "raw");
                                //���frame
                                frame.Add("common", common);
                                frame.Add("business", business);
                                frame.Add("data", data);

                                //Console.WriteLine(frame.ToString());
                                var frameData = System.Text.Encoding.UTF8.GetBytes(frame.ToString());
                                webSocket0.SendAsync(new ArraySegment<byte>(frameData), WebSocketMessageType.Text, true, cancellation);
                                Console.WriteLine("cc");
                                //webSocket.Send(JsonUtility.ToJson(frame));
                                status = StatusContinueFrame;  // �������һ֡�ı�status Ϊ 1
                                break;
                            case StatusContinueFrame:  //�м�֡status = 1
                                JObject frame1 = new JObject();
                                JObject data1 = new JObject();  //ÿһ֡��Ҫ����                                                                                                                        
                                //���data
                                data1.Add("status", StatusContinueFrame);
                                data1.Add("format", "audio/L16;rate=16000");
                                data1.Add("audio", Convert.ToBase64String(buffer));
                                data1.Add("encoding", "raw");
                                //���frame
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
                            case StatusLastFrame:    // ���һ֡��Ƶstatus = 2 ����־��Ƶ���ͽ���    
                                break;
                        }
                        await Task.Delay(intervel); //ģ����Ƶ������ʱ
                    }

                    #region ����
                    // Console.WriteLine("׼���������һ��");
                    JObject frame2 = new JObject();
                    JObject data2 = new JObject();  //ÿһ֡��Ҫ����                                                                                                                        
                                                    //���data
                    data2.Add("status", StatusLastFrame);
                    //���frame
                    frame2.Add("data", data2);

                    var frameData2 = System.Text.Encoding.UTF8.GetBytes(frame2.ToString());
                    webSocket0.SendAsync(new ArraySegment<byte>(frameData2), WebSocketMessageType.Text, true, cancellation);
                    // Console.WriteLine("�������һ�ν���");

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

        // ����codeΪ������ʱ�����ѯhttps://www.xfyun.cn/document/error-code�������
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
        /// �Ӵ�ʵ������������
        /// </summary>
        /// <param name="source">Ҫ����������</param>
        /// <param name="startIndex">��ʼ������</param>
        /// <param name="length">������󳤶�</param>
        /// <returns>���ʵ������ startIndex ����ͷ������Ϊ length ���������Ч��һ������</returns>
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