using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  红包详细信息
	/// </summary>
	[AdvancedInspector.Descriptor(" 红包详细信息", " 红包详细信息")]
	public class RedPacketDetail : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  基础信息
		/// </summary>
		[AdvancedInspector.Descriptor(" 基础信息", " 基础信息")]
		public RedPacketBaseEntry baseEntry = null;
		/// <summary>
		///  拥有者头像
		/// </summary>
		[AdvancedInspector.Descriptor(" 拥有者头像", " 拥有者头像")]
		public PlayerIcon ownerIcon = null;
		/// <summary>
		///  所有领取的玩家
		/// </summary>
		[AdvancedInspector.Descriptor(" 所有领取的玩家", " 所有领取的玩家")]
		public RedPacketReceiverEntry[] receivers = null;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			baseEntry.encode(buffer, ref pos_);
			ownerIcon.encode(buffer, ref pos_);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)receivers.Length);
			for(int i = 0; i < receivers.Length; i++)
			{
				receivers[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			baseEntry.decode(buffer, ref pos_);
			ownerIcon.decode(buffer, ref pos_);
			UInt16 receiversCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref receiversCnt);
			receivers = new RedPacketReceiverEntry[receiversCnt];
			for(int i = 0; i < receivers.Length; i++)
			{
				receivers[i] = new RedPacketReceiverEntry();
				receivers[i].decode(buffer, ref pos_);
			}
		}


		#endregion

	}

}
