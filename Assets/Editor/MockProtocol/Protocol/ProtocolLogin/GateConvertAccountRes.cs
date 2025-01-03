using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  返回转移账号信息
	/// </summary>
	[AdvancedInspector.Descriptor(" 返回转移账号信息", " 返回转移账号信息")]
	public class GateConvertAccountRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 300323;
		public UInt32 Sequence;
		/// <summary>
		///  账号
		/// </summary>
		[AdvancedInspector.Descriptor(" 账号", " 账号")]
		public string account;
		/// <summary>
		///  密码
		/// </summary>
		[AdvancedInspector.Descriptor(" 密码", " 密码")]
		public string passwd;
		/// <summary>
		///  是否保存文件
		/// </summary>
		[AdvancedInspector.Descriptor(" 是否保存文件", " 是否保存文件")]
		public byte saveFile;
		/// <summary>
		///  是否截图
		/// </summary>
		[AdvancedInspector.Descriptor(" 是否截图", " 是否截图")]
		public byte screenShot;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			byte[] accountBytes = StringHelper.StringToUTF8Bytes(account);
			BaseDLL.encode_string(buffer, ref pos_, accountBytes, (UInt16)(buffer.Length - pos_));
			byte[] passwdBytes = StringHelper.StringToUTF8Bytes(passwd);
			BaseDLL.encode_string(buffer, ref pos_, passwdBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_int8(buffer, ref pos_, saveFile);
			BaseDLL.encode_int8(buffer, ref pos_, screenShot);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 accountLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref accountLen);
			byte[] accountBytes = new byte[accountLen];
			for(int i = 0; i < accountLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref accountBytes[i]);
			}
			account = StringHelper.BytesToString(accountBytes);
			UInt16 passwdLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref passwdLen);
			byte[] passwdBytes = new byte[passwdLen];
			for(int i = 0; i < passwdLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref passwdBytes[i]);
			}
			passwd = StringHelper.BytesToString(passwdBytes);
			BaseDLL.decode_int8(buffer, ref pos_, ref saveFile);
			BaseDLL.decode_int8(buffer, ref pos_, ref screenShot);
		}

		public UInt32 GetSequence()
		{
			return Sequence;
		}

		public void SetSequence(UInt32 sequence)
		{
			Sequence = sequence;
		}

		#endregion

	}

}
