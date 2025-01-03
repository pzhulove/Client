using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// ���߼۸��������
	/// </summary>
	[AdvancedInspector.Descriptor("���߼۸��������", "���߼۸��������")]
	public class WorldAuctionQueryItemPricesRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 603923;
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
		///  ����ƽ�����׼۸�
		/// </summary>
		[AdvancedInspector.Descriptor(" ����ƽ�����׼۸�", " ����ƽ�����׼۸�")]
		public UInt32 averagePrice;
		/// <summary>
		///  Ŀǰ���۵ļ۸���͵�ͬ������
		/// </summary>
		[AdvancedInspector.Descriptor(" Ŀǰ���۵ļ۸���͵�ͬ������", " Ŀǰ���۵ļ۸���͵�ͬ������")]
		public AuctionBaseInfo[] actionItems = null;
		/// <summary>
		///  ���ڿɼ�ƽ�����׼۸�(����Ʒ)
		/// </summary>
		[AdvancedInspector.Descriptor(" ���ڿɼ�ƽ�����׼۸�(����Ʒ)", " ���ڿɼ�ƽ�����׼۸�(����Ʒ)")]
		public UInt32 visAverPrice;
		/// <summary>
		///  最小价�?
		/// </summary>
		[AdvancedInspector.Descriptor(" 最小价�?", " 最小价�?")]
		public UInt32 minPrice;
		/// <summary>
		///  最大价�?
		/// </summary>
		[AdvancedInspector.Descriptor(" 最大价�?", " 最大价�?")]
		public UInt32 maxPrice;
		/// <summary>
		///  �Ƽ��۸�
		/// </summary>
		[AdvancedInspector.Descriptor(" �Ƽ��۸�", " �Ƽ��۸�")]
		public UInt32 recommendPrice;

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
			BaseDLL.encode_uint32(buffer, ref pos_, averagePrice);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)actionItems.Length);
			for(int i = 0; i < actionItems.Length; i++)
			{
				actionItems[i].encode(buffer, ref pos_);
			}
			BaseDLL.encode_uint32(buffer, ref pos_, visAverPrice);
			BaseDLL.encode_uint32(buffer, ref pos_, minPrice);
			BaseDLL.encode_uint32(buffer, ref pos_, maxPrice);
			BaseDLL.encode_uint32(buffer, ref pos_, recommendPrice);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref type);
			BaseDLL.decode_uint32(buffer, ref pos_, ref itemTypeId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref strengthen);
			BaseDLL.decode_uint32(buffer, ref pos_, ref averagePrice);
			UInt16 actionItemsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref actionItemsCnt);
			actionItems = new AuctionBaseInfo[actionItemsCnt];
			for(int i = 0; i < actionItems.Length; i++)
			{
				actionItems[i] = new AuctionBaseInfo();
				actionItems[i].decode(buffer, ref pos_);
			}
			BaseDLL.decode_uint32(buffer, ref pos_, ref visAverPrice);
			BaseDLL.decode_uint32(buffer, ref pos_, ref minPrice);
			BaseDLL.decode_uint32(buffer, ref pos_, ref maxPrice);
			BaseDLL.decode_uint32(buffer, ref pos_, ref recommendPrice);
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
