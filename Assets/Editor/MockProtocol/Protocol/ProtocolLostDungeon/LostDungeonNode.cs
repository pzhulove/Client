using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 楼层节点
	/// </summary>
	[AdvancedInspector.Descriptor("楼层节点", "楼层节点")]
	public class LostDungeonNode : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		/// 节点id
		/// </summary>
		[AdvancedInspector.Descriptor("节点id", "节点id")]
		public UInt32 id;
		/// <summary>
		/// 宝箱id(宝箱的话)
		/// </summary>
		[AdvancedInspector.Descriptor("宝箱id(宝箱的话)", "宝箱id(宝箱的话)")]
		public UInt32 treasChestId;
		/// <summary>
		/// 状态(枚举LostDungeonNodeState战场门, LostDungeonBoxState(宝箱))
		/// </summary>
		[AdvancedInspector.Descriptor("状态(枚举LostDungeonNodeState战场门, LostDungeonBoxState(宝箱))", "状态(枚举LostDungeonNodeState战场门, LostDungeonBoxState(宝箱))")]
		public byte state;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, id);
			BaseDLL.encode_uint32(buffer, ref pos_, treasChestId);
			BaseDLL.encode_int8(buffer, ref pos_, state);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref id);
			BaseDLL.decode_uint32(buffer, ref pos_, ref treasChestId);
			BaseDLL.decode_int8(buffer, ref pos_, ref state);
		}


		#endregion

	}

}
