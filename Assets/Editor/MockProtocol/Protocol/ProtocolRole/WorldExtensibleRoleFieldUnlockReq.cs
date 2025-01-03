using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  client->world 角色扩展栏位解锁请求
	/// </summary>
	[AdvancedInspector.Descriptor(" client->world 角色扩展栏位解锁请求", " client->world 角色扩展栏位解锁请求")]
	public class WorldExtensibleRoleFieldUnlockReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 608701;
		public UInt32 Sequence;
		/// <summary>
		///  需要消耗的道具
		/// </summary>
		[AdvancedInspector.Descriptor(" 需要消耗的道具", " 需要消耗的道具")]
		public UInt64 costItemUId;

		public UInt32 costItemDataId;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, costItemUId);
			BaseDLL.encode_uint32(buffer, ref pos_, costItemDataId);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref costItemUId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref costItemDataId);
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
