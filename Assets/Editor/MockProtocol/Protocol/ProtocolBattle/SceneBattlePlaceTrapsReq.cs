using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  吃鸡放置陷阱请求
	/// </summary>
	[AdvancedInspector.Descriptor(" 吃鸡放置陷阱请求", " 吃鸡放置陷阱请求")]
	public class SceneBattlePlaceTrapsReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 508933;
		public UInt32 Sequence;

		public UInt64 itemGuid;

		public UInt32 itemCount;

		public UInt32 x;

		public UInt32 y;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, itemGuid);
			BaseDLL.encode_uint32(buffer, ref pos_, itemCount);
			BaseDLL.encode_uint32(buffer, ref pos_, x);
			BaseDLL.encode_uint32(buffer, ref pos_, y);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref itemGuid);
			BaseDLL.decode_uint32(buffer, ref pos_, ref itemCount);
			BaseDLL.decode_uint32(buffer, ref pos_, ref x);
			BaseDLL.decode_uint32(buffer, ref pos_, ref y);
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
