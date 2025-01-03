using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  吃鸡通知踩中陷阱
	/// </summary>
	[AdvancedInspector.Descriptor(" 吃鸡通知踩中陷阱", " 吃鸡通知踩中陷阱")]
	public class SceneBattleNotifyBeTraped : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 508932;
		public UInt32 Sequence;

		public UInt32 battleID;

		public UInt64 playerID;

		public UInt64 ownerID;

		public UInt32 x;

		public UInt32 y;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, battleID);
			BaseDLL.encode_uint64(buffer, ref pos_, playerID);
			BaseDLL.encode_uint64(buffer, ref pos_, ownerID);
			BaseDLL.encode_uint32(buffer, ref pos_, x);
			BaseDLL.encode_uint32(buffer, ref pos_, y);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref battleID);
			BaseDLL.decode_uint64(buffer, ref pos_, ref playerID);
			BaseDLL.decode_uint64(buffer, ref pos_, ref ownerID);
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
