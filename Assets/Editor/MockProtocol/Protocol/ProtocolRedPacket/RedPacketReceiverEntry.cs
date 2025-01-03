using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  红包领取者信息
	/// </summary>
	[AdvancedInspector.Descriptor(" 红包领取者信息", " 红包领取者信息")]
	public class RedPacketReceiverEntry : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  icon
		/// </summary>
		[AdvancedInspector.Descriptor(" icon", " icon")]
		public PlayerIcon icon = null;
		/// <summary>
		///  获得金额
		/// </summary>
		[AdvancedInspector.Descriptor(" 获得金额", " 获得金额")]
		public UInt32 money;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			icon.encode(buffer, ref pos_);
			BaseDLL.encode_uint32(buffer, ref pos_, money);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			icon.decode(buffer, ref pos_);
			BaseDLL.decode_uint32(buffer, ref pos_, ref money);
		}


		#endregion

	}

}
