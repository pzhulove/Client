using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 楼层数据
	/// </summary>
	[AdvancedInspector.Descriptor("楼层数据", "楼层数据")]
	public class LostDungeonFloorData : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		/// 第几层
		/// </summary>
		[AdvancedInspector.Descriptor("第几层", "第几层")]
		public UInt32 floor;
		/// <summary>
		/// 生成的节点
		/// </summary>
		[AdvancedInspector.Descriptor("生成的节点", "生成的节点")]
		public LostDungeonNode[] nodes = null;
		/// <summary>
		/// 楼层状态(枚举LostDungeonFloorState)
		/// </summary>
		[AdvancedInspector.Descriptor("楼层状态(枚举LostDungeonFloorState)", "楼层状态(枚举LostDungeonFloorState)")]
		public byte state;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, floor);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)nodes.Length);
			for(int i = 0; i < nodes.Length; i++)
			{
				nodes[i].encode(buffer, ref pos_);
			}
			BaseDLL.encode_int8(buffer, ref pos_, state);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref floor);
			UInt16 nodesCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref nodesCnt);
			nodes = new LostDungeonNode[nodesCnt];
			for(int i = 0; i < nodes.Length; i++)
			{
				nodes[i] = new LostDungeonNode();
				nodes[i].decode(buffer, ref pos_);
			}
			BaseDLL.decode_int8(buffer, ref pos_, ref state);
		}


		#endregion

	}

}
