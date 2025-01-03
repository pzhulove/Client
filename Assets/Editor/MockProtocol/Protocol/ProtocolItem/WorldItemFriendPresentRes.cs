using System;
using System.Text;

namespace Mock.Protocol
{

	public class WorldItemFriendPresentRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 609704;
		public UInt32 Sequence;
		/// <summary>
		/// 返回码
		/// </summary>
		[AdvancedInspector.Descriptor("返回码", "返回码")]
		public UInt32 retCode;
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
		///  更新的赠送信息
		/// </summary>
		[AdvancedInspector.Descriptor(" 更新的赠送信息", " 更新的赠送信息")]
		public FriendPresentInfo presentInfos = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, retCode);
			BaseDLL.encode_uint64(buffer, ref pos_, itemId);
			BaseDLL.encode_uint32(buffer, ref pos_, itemTypeId);
			presentInfos.encode(buffer, ref pos_);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
			BaseDLL.decode_uint64(buffer, ref pos_, ref itemId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref itemTypeId);
			presentInfos.decode(buffer, ref pos_);
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
