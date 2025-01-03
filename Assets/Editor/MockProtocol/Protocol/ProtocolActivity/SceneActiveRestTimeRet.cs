using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 查询七日活动剩余时间返回
	/// </summary>
	[AdvancedInspector.Descriptor("查询七日活动剩余时间返回", "查询七日活动剩余时间返回")]
	public class SceneActiveRestTimeRet : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501137;
		public UInt32 Sequence;

		public UInt32 time1;

		public UInt32 time2;

		public UInt32 time3;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, time1);
			BaseDLL.encode_uint32(buffer, ref pos_, time2);
			BaseDLL.encode_uint32(buffer, ref pos_, time3);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref time1);
			BaseDLL.decode_uint32(buffer, ref pos_, ref time2);
			BaseDLL.decode_uint32(buffer, ref pos_, ref time3);
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
