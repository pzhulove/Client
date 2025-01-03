using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  �Ĵ���Ʒ
	/// </summary>
	[AdvancedInspector.Descriptor(" �Ĵ���Ʒ", " �Ĵ���Ʒ")]
	public class AuctionMagicOnsale : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  �ȼ�
		/// </summary>
		[AdvancedInspector.Descriptor(" �ȼ�", " �ȼ�")]
		public byte strength;
		/// <summary>
		///  ����
		/// </summary>
		[AdvancedInspector.Descriptor(" ����", " ����")]
		public UInt32 num;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, strength);
			BaseDLL.encode_uint32(buffer, ref pos_, num);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref strength);
			BaseDLL.decode_uint32(buffer, ref pos_, ref num);
		}


		#endregion

	}

}
