using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  请求复活
	/// </summary>
	[AdvancedInspector.Descriptor(" 请求复活", " 请求复活")]
	public class SceneDungeonReviveReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 506817;
		public UInt32 Sequence;
		/// <summary>
		///  要复活的目标
		/// </summary>
		[AdvancedInspector.Descriptor(" 要复活的目标", " 要复活的目标")]
		public UInt64 targetId;
		/// <summary>
		///  每一次复活都有一个ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 每一次复活都有一个ID", " 每一次复活都有一个ID")]
		public UInt32 reviveId;
		/// <summary>
		///  消耗的复活币数量
		/// </summary>
		[AdvancedInspector.Descriptor(" 消耗的复活币数量", " 消耗的复活币数量")]
		public UInt32 reviveCoinNum;
		/// <summary>
		///  复活道具
		/// </summary>
		[AdvancedInspector.Descriptor(" 复活道具", " 复活道具")]
		public UInt32 reviveItem;
		/// <summary>
		///  道具数量
		/// </summary>
		[AdvancedInspector.Descriptor(" 道具数量", " 道具数量")]
		public UInt32 reviveItemNum;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, targetId);
			BaseDLL.encode_uint32(buffer, ref pos_, reviveId);
			BaseDLL.encode_uint32(buffer, ref pos_, reviveCoinNum);
			BaseDLL.encode_uint32(buffer, ref pos_, reviveItem);
			BaseDLL.encode_uint32(buffer, ref pos_, reviveItemNum);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref targetId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref reviveId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref reviveCoinNum);
			BaseDLL.decode_uint32(buffer, ref pos_, ref reviveItem);
			BaseDLL.decode_uint32(buffer, ref pos_, ref reviveItemNum);
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
