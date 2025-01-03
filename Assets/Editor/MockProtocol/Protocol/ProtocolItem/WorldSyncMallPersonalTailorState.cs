using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 同步商城私人订制
	/// </summary>
	[AdvancedInspector.Descriptor("同步商城私人订制", "同步商城私人订制")]
	public class WorldSyncMallPersonalTailorState : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 602818;
		public UInt32 Sequence;
		/// <summary>
		/// 对应枚举MallGiftPackActivateCond
		/// </summary>
		[AdvancedInspector.Descriptor("对应枚举MallGiftPackActivateCond", "对应枚举MallGiftPackActivateCond")]
		public byte state;
		/// <summary>
		/// 最新触发的商品id
		/// </summary>
		[AdvancedInspector.Descriptor("最新触发的商品id", "最新触发的商品id")]
		public UInt32 goodsId;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, state);
			BaseDLL.encode_uint32(buffer, ref pos_, goodsId);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref state);
			BaseDLL.decode_uint32(buffer, ref pos_, ref goodsId);
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
