using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// ------------商城相关-----------------------
	/// </summary>
	/// <summary>
	/// 激活商城限时礼包请求
	/// </summary>
	[AdvancedInspector.Descriptor("激活商城限时礼包请求", "激活商城限时礼包请求")]
	public class WorldMallGiftPackActivateReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 602814;
		public UInt32 Sequence;
		/// <summary>
		///  对应枚举MallGiftPackActivateCond
		/// </summary>
		[AdvancedInspector.Descriptor(" 对应枚举MallGiftPackActivateCond", " 对应枚举MallGiftPackActivateCond")]
		public byte giftPackActCond;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, giftPackActCond);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref giftPackActCond);
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
