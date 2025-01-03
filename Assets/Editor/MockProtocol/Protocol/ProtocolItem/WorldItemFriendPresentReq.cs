using System;
using System.Text;

namespace Mock.Protocol
{

	public class WorldItemFriendPresentReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 609703;
		public UInt32 Sequence;
		/// <summary>
		///  道具guid
		/// </summary>
		[AdvancedInspector.Descriptor(" 道具guid", " 道具guid")]
		public UInt64 itemId;
		/// <summary>
		///  道具类型id
		/// </summary>
		[AdvancedInspector.Descriptor(" 道具类型id", " 道具类型id")]
		public UInt32 itemTypeId;
		/// <summary>
		///  赠送好友id
		/// </summary>
		[AdvancedInspector.Descriptor(" 赠送好友id", " 赠送好友id")]
		public UInt64 friendId;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, itemId);
			BaseDLL.encode_uint32(buffer, ref pos_, itemTypeId);
			BaseDLL.encode_uint64(buffer, ref pos_, friendId);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref itemId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref itemTypeId);
			BaseDLL.decode_uint64(buffer, ref pos_, ref friendId);
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
