using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// ��ѯ���߼۸����(���ڽ���ƽ����, ����������ڳ��ۼ۸�)
	/// </summary>
	[AdvancedInspector.Descriptor("��ѯ���߼۸����(���ڽ���ƽ����, ����������ڳ��ۼ۸�)", "��ѯ���߼۸����(���ڽ���ƽ����, ����������ڳ��ۼ۸�)")]
	public class WorldAuctionQueryItemPricesReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 603922;
		public UInt32 Sequence;
		/// <summary>
		///  ����������
		/// </summary>
		[AdvancedInspector.Descriptor(" ����������", " ����������")]
		public byte type;
		/// <summary>
		///  ��Ʒ����id
		/// </summary>
		[AdvancedInspector.Descriptor(" ��Ʒ����id", " ��Ʒ����id")]
		public UInt32 itemTypeId;
		/// <summary>
		///  ��Ʒǿ���ȼ�
		/// </summary>
		[AdvancedInspector.Descriptor(" ��Ʒǿ���ȼ�", " ��Ʒǿ���ȼ�")]
		public UInt32 strengthen;
		/// <summary>
		///  ���鸽��buffid
		/// </summary>
		[AdvancedInspector.Descriptor(" ���鸽��buffid", " ���鸽��buffid")]
		public UInt32 beadbuffid;
		/// <summary>
		/// ����װ����������:��Ч0/����1/����2/����3/����4
		/// </summary>
		[AdvancedInspector.Descriptor("����װ����������:��Ч0/����1/����2/����3/����4", "����װ����������:��Ч0/����1/����2/����3/����4")]
		public byte enhanceType;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, type);
			BaseDLL.encode_uint32(buffer, ref pos_, itemTypeId);
			BaseDLL.encode_uint32(buffer, ref pos_, strengthen);
			BaseDLL.encode_uint32(buffer, ref pos_, beadbuffid);
			BaseDLL.encode_int8(buffer, ref pos_, enhanceType);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref type);
			BaseDLL.decode_uint32(buffer, ref pos_, ref itemTypeId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref strengthen);
			BaseDLL.decode_uint32(buffer, ref pos_, ref beadbuffid);
			BaseDLL.decode_int8(buffer, ref pos_, ref enhanceType);
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
