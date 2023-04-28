
using System;

using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;  //����Ҫ��Ӹ�����

using System.IO;
using System.Text;
using System.Threading;  //���������ռ�,��Thread���ڴ˿ռ���
using System.Threading.Tasks;



using System.Net;
using System.Security.Cryptography;

using System.Collections;
using Newtonsoft.Json;//
					  //using NAudio.Wave;
using System.Net.WebSockets;
using Newtonsoft.Json.Linq;//
using MACRODEFIND;

namespace MACRODEFIND
{
	class CDefineinfo
    {
		public enum enSdkCbType
		{
			CB_Asw_OpenTermAudVid = 101, // Ӧ����ն���Ƶ��Ƶ
			CB_Asw_CloseTermAudVid = 102, // Ӧ��ر��ն���Ƶ��Ƶ
			CB_Asw_OpenTermTalk = 103, // Ӧ����ն˶Խ�
			CB_Asw_CloseTermTalk = 104, // Ӧ��ر��ն˶Խ�
			CB_Asw_GetAudLevel = 105, // Ӧ���ȡ�ն���������
			CB_Asw_SetAudLevel = 106, // Ӧ�������ն���������
			CB_Asw_DismissTerm = 107, // Ӧ�����ֻ�����
			CB_Asw_GetDbAlmCfg = 108, // Ӧ���ȡ�ն�������������
			CB_Asw_SetDbAlmCfg = 109, // Ӧ�������ն�������������
			CB_Asw_NvrSrchRecFile = 110, // Ӧ��NVR����¼���ļ����
			CB_Asw_NvrPlayRecFile = 111, // Ӧ��NVR�ط�¼���ļ����

			CB_Post_TermSos = 201, // �����ն˺���
			CB_Post_TermState = 202, // �����ն�״̬
			CB_Post_485PipeData = 203, // ����485�ܵ�����(����)
			CB_Post_Mp3PlayFinish = 204, // ����MP3�ļ����Ž���֪ͨ
			CB_Post_AlmNotify = 205, // ���ͱ���֪ͨ
			CB_Post_NvrChanNotify = 206, // ����NVRͨ��֪ͨ
			CB_Post_TermDbValL = 207, // ��������ʰ������һ��������ֵ
			CB_Post_TermDbValR = 208, // ��������ʰ�����ڶ���������ֵ
			CB_Post_NvrSrchRecFile = 209, // ����NVR����¼���ļ��б���Ϣ
			CB_Post_NvrPlayProg = 210, // ����NVR¼���ļ��طŽ���

			CB_Event_TermRegister = 301, // �ն�����ע��֪ͨ������-1����ע��ʧ�ܣ�����0����ע��ɹ�
			CB_Event_TermConnect = 302, // �ն���������֪ͨ������-1�����������ӣ�����0������������
			CB_Event_TermCnnLost = 303, // �ն˶Ͽ�����֪ͨ

			CB_Data_TermAudio = 401, // �ն˵���Ƶ���Ѿ�ѹ�����ģ���Ҫ����SDK���Žӿڲ��ܲ���
			CB_Data_TermVideo = 402, // �ն˵�H264��׼��Ƶ֡�����Ե���SDK���Žӿڲ���
			CB_Data_PcMicAudio = 403, // ���Ե���Ƶ��ΪPCM֡��8K�����ʣ�16λ/��
			CB_Data_BypassAudio = 404, // ��·��Ƶ��ΪPCM֡��8K�����ʣ�16λ/��
			CB_Data_TermMp3L = 405, // ����ʰ������һ����MP3��Ƶ
			CB_Data_TermMp3R = 406, // ����ʰ�����ڶ�����MP3��Ƶ
			CB_Data_TermPcmL = 407, // ����ʰ������һ����PCM��Ƶ(16K������16λ)
			CB_Data_TermPcmR = 408, // ����ʰ�����ڶ�����PCM��Ƶ(16K������16λ)
			CB_Data_NvrMp3 = 409, // ¼������MP3��Ƶ
			CB_Data_NvrPcm = 410, // ¼������PCM��Ƶ(16K������16λ)
			CB_Data_NvrPlayMp3 = 411, // ¼�������ط�MP3��Ƶ

		};

		public enum enSdkDevType
		{
			TSDK_DEV_TYPE_UNDEF = 0,  // δ֪�豸����
			TSDK_DEV_TYPE_TALK = 1,  // �Խ��ն� �� �Խ��㲥�ն�
			TSDK_DEV_TYPE_BROAD = 2,  // �㲥�ն�
			TSDK_DEV_TYPE_MP3 = 3,  // ����ʰ����
			TSDK_DEV_TYPE_NVR = 4,  // ¼������
		};

		public enum enSdkDevState
		{
			TSDK_DEV_STATE_OFFLINE = 0,  // ����
			TSDK_DEV_STATE_ONLINE = 1,  // ����
			TSDK_DEV_STATE_PLAYING = 2,  // ���ڲ���mp3
			TSDK_DEV_STATE_TALKING = 3,  // ���ڶԽ�
			TSDK_DEV_STATE_TALK_PLAY = 4   // ���ڶԽ��Ͳ���mp3
		};


		public unsafe struct TSdkDataTermMp3L
		{
			public ulong dwTermID;
			//[MarshalAs(UnmanagedType.LPArray)] public byte[] pMp3Data;
			public byte* pMp3Data;
			//[MarshalAs(UnmanagedType.LPArray)] Intptr
			//[MarshalAs(UnmanagedType.LPArray)] Intptr pMp3Data;
			public int nDataSize;
		};

		public struct TSdkEventTermRegister
		{
			public enSdkDevType eTermType;
			public uint dwTermID;    // �ն��豸ID�ţ���16λΪ�����ù��ߡ�ע���豸�ġ�����š�(1��10)����16λΪ�����ù��ߡ�ע���豸�ġ��豸�š�(1��600)
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
			public byte[] TermMac;   // �ն�MAC��ַ
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
			public char[] TermIp;   // �ն�IP��ַ
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
			public char[] TermName; // �ն�����
		};

		public struct TSdkPostTermState
		{
			public uint dwTermID;
			public enSdkDevState eTermState;
			public byte AlmInMask;   // ��������ͨ����Чָʾ�룬ÿλ��Ӧһ·����: 0(ͨ����Ч), 1(ͨ����Ч)����ʱֻ֧��2·�������룬��1λΪͨ��1����2λΪͨ��2��
			public byte AlmInState;  // ��������ͨ��״ָ̬ʾ�룬ÿλ��Ӧһ·����: 0(�ޱ���), 1(�б���)��
			public byte AlmOutMask;  // �������ͨ����Чָʾ�룬ÿλ��Ӧһ·����: 0(ͨ����Ч), 1(ͨ����Ч)����ʱֻ֧��2·�����������1λΪͨ��1����2λΪͨ��2��
			public byte AlmOutState; // �������ͨ��״ָ̬ʾ�룬ÿλ��Ӧһ·����: 0(�ޱ���), 1(�б���)��
		};

		static public enSdkDevType eTermType;
		static public ulong dwTermID;    // �ն��豸ID�ţ���16λΪ�����ù��ߡ�ע���豸�ġ�����š�(1��10)����16λΪ�����ù��ߡ�ע���豸�ġ��豸�š�(1��600)
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
		static public byte[] TermMac;   // �ն�MAC��ַ
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		static public char[] TermIp;   // �ն�IP��ַ
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
		static public char[] TermName; // �ն�����

		static public bool bHasVideo;    // TRUE:����Ƶ����Ƶ , FALSE:����Ƶֻ����Ƶ
		static public bool bTalkbackEnable; // ֻ�������ʰ������TRUE:�����Խ�ģʽ��FALSE:�رնԽ�ģʽ

		//[StructLayout(layoutKind: LayoutKind.Sequential, Pack = 1)]
		public struct TSdkEventTermConnect
		{
			public enSdkDevType eTermType;
			public uint dwTermID;    // �ն��豸ID�ţ���16λΪ�����ù��ߡ�ע���豸�ġ�����š�(1��10)����16λΪ�����ù��ߡ�ע���豸�ġ��豸�š�(1��600)
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
			public byte[] TermMac;   // �ն�MAC��ַ
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
			public byte[] TermIp;   // �ն�IP��ַ
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
			public byte[] TermName; // �ն�����

			//public bool bHasVideo;    // TRUE:����Ƶ����Ƶ , FALSE:����Ƶֻ����Ƶ
			//public bool bTalkbackEnable; // ֻ�������ʰ������TRUE:�����Խ�ģʽ��FALSE:�رնԽ�ģʽ
			public uint bHasVideo;    // TRUE:����Ƶ����Ƶ , FALSE:����Ƶֻ����Ƶ
			public uint bTalkbackEnable; // ֻ�������ʰ������TRUE:�����Խ�ģʽ��FALSE:�رնԽ�ģʽ
		};

		public struct TSdkAswOpenTermAudVid
		{
			public int nResult; // ���������ɹ�Ϊ0������Ϊ�������:enSdkErrCode
			public uint dwTermID;
		};

		public unsafe struct TSdkDataTermMp3R
		{
			public uint dwTermID;
			public byte* pMp3Data;

			//public byte[] pMp3Data;
			public int nDataSize;
		};
	}
	
}