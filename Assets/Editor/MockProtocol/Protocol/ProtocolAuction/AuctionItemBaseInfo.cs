using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  �����е��߻�����Ϣ�����ͣ�������
	/// </summary>
	[AdvancedInspector.Descriptor(" �����е��߻�����Ϣ�����ͣ�������", " �����е��߻�����Ϣ�����ͣ�������")]
	public class AuctionItemBaseInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  ����ID
		/// </summary>
		[AdvancedInspector.Descriptor(" ����ID", " ����ID")]
		public UInt32 itemTypeId;
		/// <summary>
		///  ��������
		/// </summary>
		[AdvancedInspector.Descriptor(" ��������", " ��������")]
		public UInt32 num;
		/// <summary>
		///  �Ƿ�����Ʒ
		/// </summary>
		[AdvancedInspector.Descriptor(" �Ƿ�����Ʒ", " �Ƿ�����Ʒ")]
		public byte isTreas;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, itemTypeId);
			BaseDLL.encode_uint32(buffer, ref pos_, num);
			BaseDLL.encode_int8(buffer, ref pos_, isTreas);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref itemTypeId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref num);
			BaseDLL.decode_int8(buffer, ref pos_, ref isTreas);
		}


		#endregion

	}

}
