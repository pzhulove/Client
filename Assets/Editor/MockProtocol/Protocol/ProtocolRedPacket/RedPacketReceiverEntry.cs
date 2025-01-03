using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  �����ȡ����Ϣ
	/// </summary>
	[AdvancedInspector.Descriptor(" �����ȡ����Ϣ", " �����ȡ����Ϣ")]
	public class RedPacketReceiverEntry : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  icon
		/// </summary>
		[AdvancedInspector.Descriptor(" icon", " icon")]
		public PlayerIcon icon = null;
		/// <summary>
		///  ��ý��
		/// </summary>
		[AdvancedInspector.Descriptor(" ��ý��", " ��ý��")]
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
