using System;
using System.Text;

namespace Mock.Protocol
{

	public class SceneDungeonChestNotify : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 506816;
		public UInt32 Sequence;
		/// <summary>
		///  宝箱付费货币类型
		/// </summary>
		[AdvancedInspector.Descriptor(" 宝箱付费货币类型", " 宝箱付费货币类型")]
		public UInt32 payChestCostItemId;
		/// <summary>
		///  宝箱付费货币数量
		/// </summary>
		[AdvancedInspector.Descriptor(" 宝箱付费货币数量", " 宝箱付费货币数量")]
		public UInt32 payChestCost;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, payChestCostItemId);
			BaseDLL.encode_uint32(buffer, ref pos_, payChestCost);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref payChestCostItemId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref payChestCost);
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
