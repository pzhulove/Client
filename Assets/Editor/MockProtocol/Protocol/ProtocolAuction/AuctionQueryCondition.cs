using System;
using System.Text;

namespace Mock.Protocol
{

	public class AuctionQueryCondition : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  ��������
		/// </summary>
		[AdvancedInspector.Descriptor(" ��������", " ��������")]
		public byte type;
		/// <summary>
		///  ��Ʒ״̬(AuctionGoodState)
		/// </summary>
		[AdvancedInspector.Descriptor(" ��Ʒ״̬(AuctionGoodState)", " ��Ʒ״̬(AuctionGoodState)")]
		public byte goodState;
		/// <summary>
		///  ��Ʒ������(AuctionMainItemType)
		/// </summary>
		[AdvancedInspector.Descriptor(" ��Ʒ������(AuctionMainItemType)", " ��Ʒ������(AuctionMainItemType)")]
		public byte itemMainType;
		/// <summary>
		///  ��Ʒ������
		/// </summary>
		[AdvancedInspector.Descriptor(" ��Ʒ������", " ��Ʒ������")]
		public UInt32[] itemSubTypes = new UInt32[0];
		/// <summary>
		///  �ų���Ʒ������
		/// </summary>
		[AdvancedInspector.Descriptor(" �ų���Ʒ������", " �ų���Ʒ������")]
		public UInt32[] excludeItemSubTypes = new UInt32[0];
		/// <summary>
		///  ��ƷID
		/// </summary>
		[AdvancedInspector.Descriptor(" ��ƷID", " ��ƷID")]
		public UInt32 itemTypeID;
		/// <summary>
		///  Ʒ��
		/// </summary>
		[AdvancedInspector.Descriptor(" Ʒ��", " Ʒ��")]
		public byte quality;
		/// <summary>
		///  �����Ʒ�ȼ�
		/// </summary>
		[AdvancedInspector.Descriptor(" �����Ʒ�ȼ�", " �����Ʒ�ȼ�")]
		public byte minLevel;
		/// <summary>
		///  �����Ʒ�ȼ�
		/// </summary>
		[AdvancedInspector.Descriptor(" �����Ʒ�ȼ�", " �����Ʒ�ȼ�")]
		public byte maxLevel;
		/// <summary>
		///  ���ǿ���ȼ�
		/// </summary>
		[AdvancedInspector.Descriptor(" ���ǿ���ȼ�", " ���ǿ���ȼ�")]
		public byte minStrength;
		/// <summary>
		///  ���ǿ���ȼ�
		/// </summary>
		[AdvancedInspector.Descriptor(" ���ǿ���ȼ�", " ���ǿ���ȼ�")]
		public byte maxStrength;
		/// <summary>
		///  ����ʽ����Ӧö��AuctionSortType��
		/// </summary>
		[AdvancedInspector.Descriptor(" ����ʽ����Ӧö��AuctionSortType��", " ����ʽ����Ӧö��AuctionSortType��")]
		public byte sortType;
		/// <summary>
		///  ҳ��
		/// </summary>
		[AdvancedInspector.Descriptor(" ҳ��", " ҳ��")]
		public UInt16 page;
		/// <summary>
		///  ÿҳ��Ʒ����
		/// </summary>
		[AdvancedInspector.Descriptor(" ÿҳ��Ʒ����", " ÿҳ��Ʒ����")]
		public byte itemNumPerPage;
		/// <summary>
		///  ǿ����ǿ���ȼ�
		/// </summary>
		[AdvancedInspector.Descriptor(" ǿ����ǿ���ȼ�", " ǿ����ǿ���ȼ�")]
		public UInt32 couponStrengthToLev;
		/// <summary>
		///  �Ƿ��ע[0]�ǹ�ע,[1]��ע,ö��(AuctionAttentType)
		/// </summary>
		[AdvancedInspector.Descriptor(" �Ƿ��ע[0]�ǹ�ע,[1]��ע,ö��(AuctionAttentType)", " �Ƿ��ע[0]�ǹ�ע,[1]��ע,ö��(AuctionAttentType)")]
		public byte attent;
		/// <summary>
		///  ְҵ
		/// </summary>
		[AdvancedInspector.Descriptor(" ְҵ", " ְҵ")]
		public byte[] occus = new byte[0];

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, type);
			BaseDLL.encode_int8(buffer, ref pos_, goodState);
			BaseDLL.encode_int8(buffer, ref pos_, itemMainType);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)itemSubTypes.Length);
			for(int i = 0; i < itemSubTypes.Length; i++)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, itemSubTypes[i]);
			}
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)excludeItemSubTypes.Length);
			for(int i = 0; i < excludeItemSubTypes.Length; i++)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, excludeItemSubTypes[i]);
			}
			BaseDLL.encode_uint32(buffer, ref pos_, itemTypeID);
			BaseDLL.encode_int8(buffer, ref pos_, quality);
			BaseDLL.encode_int8(buffer, ref pos_, minLevel);
			BaseDLL.encode_int8(buffer, ref pos_, maxLevel);
			BaseDLL.encode_int8(buffer, ref pos_, minStrength);
			BaseDLL.encode_int8(buffer, ref pos_, maxStrength);
			BaseDLL.encode_int8(buffer, ref pos_, sortType);
			BaseDLL.encode_uint16(buffer, ref pos_, page);
			BaseDLL.encode_int8(buffer, ref pos_, itemNumPerPage);
			BaseDLL.encode_uint32(buffer, ref pos_, couponStrengthToLev);
			BaseDLL.encode_int8(buffer, ref pos_, attent);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)occus.Length);
			for(int i = 0; i < occus.Length; i++)
			{
				BaseDLL.encode_int8(buffer, ref pos_, occus[i]);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref type);
			BaseDLL.decode_int8(buffer, ref pos_, ref goodState);
			BaseDLL.decode_int8(buffer, ref pos_, ref itemMainType);
			UInt16 itemSubTypesCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref itemSubTypesCnt);
			itemSubTypes = new UInt32[itemSubTypesCnt];
			for(int i = 0; i < itemSubTypes.Length; i++)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemSubTypes[i]);
			}
			UInt16 excludeItemSubTypesCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref excludeItemSubTypesCnt);
			excludeItemSubTypes = new UInt32[excludeItemSubTypesCnt];
			for(int i = 0; i < excludeItemSubTypes.Length; i++)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref excludeItemSubTypes[i]);
			}
			BaseDLL.decode_uint32(buffer, ref pos_, ref itemTypeID);
			BaseDLL.decode_int8(buffer, ref pos_, ref quality);
			BaseDLL.decode_int8(buffer, ref pos_, ref minLevel);
			BaseDLL.decode_int8(buffer, ref pos_, ref maxLevel);
			BaseDLL.decode_int8(buffer, ref pos_, ref minStrength);
			BaseDLL.decode_int8(buffer, ref pos_, ref maxStrength);
			BaseDLL.decode_int8(buffer, ref pos_, ref sortType);
			BaseDLL.decode_uint16(buffer, ref pos_, ref page);
			BaseDLL.decode_int8(buffer, ref pos_, ref itemNumPerPage);
			BaseDLL.decode_uint32(buffer, ref pos_, ref couponStrengthToLev);
			BaseDLL.decode_int8(buffer, ref pos_, ref attent);
			UInt16 occusCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref occusCnt);
			occus = new byte[occusCnt];
			for(int i = 0; i < occus.Length; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref occus[i]);
			}
		}


		#endregion

	}

}
