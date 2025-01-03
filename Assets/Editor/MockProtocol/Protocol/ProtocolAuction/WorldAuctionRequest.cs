using System;
using System.Text;

namespace Mock.Protocol
{

	public class WorldAuctionRequest : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 603906;
		public UInt32 Sequence;
		/// <summary>
		///  ��������
		/// </summary>
		[AdvancedInspector.Descriptor(" ��������", " ��������")]
		public byte type;
		/// <summary>
		///  ��������id
		/// </summary>
		[AdvancedInspector.Descriptor(" ��������id", " ��������id")]
		public UInt64 id;
		/// <summary>
		///  ������������id
		/// </summary>
		[AdvancedInspector.Descriptor(" ������������id", " ������������id")]
		public UInt32 typeId;
		/// <summary>
		///  ����
		/// </summary>
		[AdvancedInspector.Descriptor(" ����", " ����")]
		public UInt32 num;
		/// <summary>
		///  �۸�
		/// </summary>
		[AdvancedInspector.Descriptor(" �۸�", " �۸�")]
		public UInt32 price;
		/// <summary>
		///  ����ʱ��(AuctionSellDuration)
		/// </summary>
		[AdvancedInspector.Descriptor(" ����ʱ��(AuctionSellDuration)", " ����ʱ��(AuctionSellDuration)")]
		public byte duration;
		/// <summary>
		///  ǿ���ȼ�
		/// </summary>
		[AdvancedInspector.Descriptor(" ǿ���ȼ�", " ǿ���ȼ�")]
		public byte strength;
		/// <summary>
		///  ����buffid
		/// </summary>
		[AdvancedInspector.Descriptor(" ����buffid", " ����buffid")]
		public UInt32 beadbuffId;
		/// <summary>
		///  �Ƿ������ϼ�
		/// </summary>
		[AdvancedInspector.Descriptor(" �Ƿ������ϼ�", " �Ƿ������ϼ�")]
		public byte isAgain;
		/// <summary>
		///  �����ϼ�guid
		/// </summary>
		[AdvancedInspector.Descriptor(" �����ϼ�guid", " �����ϼ�guid")]
		public UInt64 auctionGuid;
		/// <summary>
		///  ����װ���������� ��Ч0/����1/����2/����3/����4
		/// </summary>
		[AdvancedInspector.Descriptor(" ����װ���������� ��Ч0/����1/����2/����3/����4", " ����װ���������� ��Ч0/����1/����2/����3/����4")]
		public byte enhanceType;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, type);
			BaseDLL.encode_uint64(buffer, ref pos_, id);
			BaseDLL.encode_uint32(buffer, ref pos_, typeId);
			BaseDLL.encode_uint32(buffer, ref pos_, num);
			BaseDLL.encode_uint32(buffer, ref pos_, price);
			BaseDLL.encode_int8(buffer, ref pos_, duration);
			BaseDLL.encode_int8(buffer, ref pos_, strength);
			BaseDLL.encode_uint32(buffer, ref pos_, beadbuffId);
			BaseDLL.encode_int8(buffer, ref pos_, isAgain);
			BaseDLL.encode_uint64(buffer, ref pos_, auctionGuid);
			BaseDLL.encode_int8(buffer, ref pos_, enhanceType);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref type);
			BaseDLL.decode_uint64(buffer, ref pos_, ref id);
			BaseDLL.decode_uint32(buffer, ref pos_, ref typeId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref num);
			BaseDLL.decode_uint32(buffer, ref pos_, ref price);
			BaseDLL.decode_int8(buffer, ref pos_, ref duration);
			BaseDLL.decode_int8(buffer, ref pos_, ref strength);
			BaseDLL.decode_uint32(buffer, ref pos_, ref beadbuffId);
			BaseDLL.decode_int8(buffer, ref pos_, ref isAgain);
			BaseDLL.decode_uint64(buffer, ref pos_, ref auctionGuid);
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
