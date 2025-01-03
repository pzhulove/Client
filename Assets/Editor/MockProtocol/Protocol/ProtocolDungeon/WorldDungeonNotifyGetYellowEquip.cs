using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  world->client 通知地下城获得橙色装备
	/// </summary>
	[AdvancedInspector.Descriptor(" world->client 通知地下城获得橙色装备", " world->client 通知地下城获得橙色装备")]
	public class WorldDungeonNotifyGetYellowEquip : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 606820;
		public UInt32 Sequence;
		/// <summary>
		///  角色ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 角色ID", " 角色ID")]
		public UInt64 roleId;
		/// <summary>
		///  道具ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 道具ID", " 道具ID")]
		public UInt32 itemId;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, roleId);
			BaseDLL.encode_uint32(buffer, ref pos_, itemId);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref itemId);
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
