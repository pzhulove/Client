using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  上报加载进度
	/// </summary>
	[AdvancedInspector.Descriptor(" 上报加载进度", " 上报加载进度")]
	public class RelaySvrReportLoadProgress : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 1300015;
		public UInt32 Sequence;
		/// <summary>
		///  加载进度
		/// </summary>
		[AdvancedInspector.Descriptor(" 加载进度", " 加载进度")]
		public byte progress;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, progress);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref progress);
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
