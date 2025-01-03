using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  物品寄存领取到期
	/// </summary>
	[AdvancedInspector.Descriptor(" 物品寄存领取到期", " 物品寄存领取到期")]
	public class SceneItemDepositExpire : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501059;
		public UInt32 Sequence;
		/// <summary>
		///  剩余过期时间
		/// </summary>
		[AdvancedInspector.Descriptor(" 剩余过期时间", " 剩余过期时间")]
		public UInt32 oddExpireTime;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, oddExpireTime);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref oddExpireTime);
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
