using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  同一背包移动道具到指定位置
	/// </summary>
	[AdvancedInspector.Descriptor(" 同一背包移动道具到指定位置", " 同一背包移动道具到指定位置")]
	public class SceneMoveItem : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 500962;
		public UInt32 Sequence;
		/// <summary>
		///  道具guid
		/// </summary>
		[AdvancedInspector.Descriptor(" 道具guid", " 道具guid")]
		public UInt64 itemId;
		/// <summary>
		///  道具数量
		/// </summary>
		[AdvancedInspector.Descriptor(" 道具数量", " 道具数量")]
		public UInt16 num;
		/// <summary>
		///  道具目标位置
		/// </summary>
		[AdvancedInspector.Descriptor(" 道具目标位置", " 道具目标位置")]
		public ItemPos targetPos = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, itemId);
			BaseDLL.encode_uint16(buffer, ref pos_, num);
			targetPos.encode(buffer, ref pos_);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref itemId);
			BaseDLL.decode_uint16(buffer, ref pos_, ref num);
			targetPos.decode(buffer, ref pos_);
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
