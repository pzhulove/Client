using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  自定义日志上报
	/// </summary>
	[AdvancedInspector.Descriptor(" 自定义日志上报", " 自定义日志上报")]
	public class SceneCustomLogReport : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 503402;
		public UInt32 Sequence;
		/// <summary>
		///  对应枚举 CustomLogReportType
		/// </summary>
		[AdvancedInspector.Descriptor(" 对应枚举 CustomLogReportType", " 对应枚举 CustomLogReportType")]
		public byte type;
		/// <summary>
		///  变量
		/// </summary>
		[AdvancedInspector.Descriptor(" 变量", " 变量")]
		public string param;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, type);
			byte[] paramBytes = StringHelper.StringToUTF8Bytes(param);
			BaseDLL.encode_string(buffer, ref pos_, paramBytes, (UInt16)(buffer.Length - pos_));
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref type);
			UInt16 paramLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref paramLen);
			byte[] paramBytes = new byte[paramLen];
			for(int i = 0; i < paramLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref paramBytes[i]);
			}
			param = StringHelper.BytesToString(paramBytes);
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
