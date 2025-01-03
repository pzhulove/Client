using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  �����ϸ��Ϣ
	/// </summary>
	[AdvancedInspector.Descriptor(" �����ϸ��Ϣ", " �����ϸ��Ϣ")]
	public class RedPacketDetail : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  ������Ϣ
		/// </summary>
		[AdvancedInspector.Descriptor(" ������Ϣ", " ������Ϣ")]
		public RedPacketBaseEntry baseEntry = null;
		/// <summary>
		///  ӵ����ͷ��
		/// </summary>
		[AdvancedInspector.Descriptor(" ӵ����ͷ��", " ӵ����ͷ��")]
		public PlayerIcon ownerIcon = null;
		/// <summary>
		///  ������ȡ�����
		/// </summary>
		[AdvancedInspector.Descriptor(" ������ȡ�����", " ������ȡ�����")]
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
