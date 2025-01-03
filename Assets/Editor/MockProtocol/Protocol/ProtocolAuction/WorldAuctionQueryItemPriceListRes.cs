using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// world->client ���߼۸��������
	/// </summary>
	[AdvancedInspector.Descriptor("world->client ���߼۸��������", "world->client ���߼۸��������")]
	public class WorldAuctionQueryItemPriceListRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 603932;
		public UInt32 Sequence;
		/// <summary>
		///  ����������
		/// </summary>
		[AdvancedInspector.Descriptor(" ����������", " ����������")]
		public byte type;
		/// <summary>
		///  ��Ʒ����״̬ [1]:��ʾ [2]:�ϼ�
		/// </summary>
		[AdvancedInspector.Descriptor(" ��Ʒ����״̬ [1]:��ʾ [2]:�ϼ�", " ��Ʒ����״̬ [1]:��ʾ [2]:�ϼ�")]
		public byte auctionState;
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
		///  Ŀǰ���۵ļ۸���͵�ͬ������
		/// </summary>
		[AdvancedInspector.Descriptor(" Ŀǰ���۵ļ۸���͵�ͬ������", " Ŀǰ���۵ļ۸���͵�ͬ������")]
		public AuctionBaseInfo[] actionItems = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, type);
			BaseDLL.encode_int8(buffer, ref pos_, auctionState);
			BaseDLL.encode_uint32(buffer, ref pos_, itemTypeId);
			BaseDLL.encode_uint32(buffer, ref pos_, strengthen);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)actionItems.Length);
			for(int i = 0; i < actionItems.Length; i++)
			{
				actionItems[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref type);
			BaseDLL.decode_int8(buffer, ref pos_, ref auctionState);
			BaseDLL.decode_uint32(buffer, ref pos_, ref itemTypeId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref strengthen);
			UInt16 actionItemsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref actionItemsCnt);
			actionItems = new AuctionBaseInfo[actionItemsCnt];
			for(int i = 0; i < actionItems.Length; i++)
			{
				actionItems[i] = new AuctionBaseInfo();
				actionItems[i].decode(buffer, ref pos_);
			}
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
