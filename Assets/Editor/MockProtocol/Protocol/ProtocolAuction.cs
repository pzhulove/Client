using System;
using System.Text;

namespace Mock.Protocol
{

	public enum AuctionType
	{

		Item = 0,

		Gold = 1,
	}


	public enum AuctionSortType
	{
		/// <summary>
		///  ���۸�����
		/// </summary>
		[AdvancedInspector.Descriptor(" ���۸�����", " ���۸�����")]
		PriceAsc = 0,
		/// <summary>
		///  ���۸���
		/// </summary>
		[AdvancedInspector.Descriptor(" ���۸���", " ���۸���")]
		PriceDesc = 1,
	}


	public enum AuctionSellDuration
	{
		/// <summary>
		///  24Сʱ
		/// </summary>
		[AdvancedInspector.Descriptor(" 24Сʱ", " 24Сʱ")]
		Hour_24 = 0,
		/// <summary>
		///  48Сʱ
		/// </summary>
		[AdvancedInspector.Descriptor(" 48Сʱ", " 48Сʱ")]
		Hour_48 = 1,
	}


	public enum AuctionMainItemType
	{
		/// <summary>
		///  ��Ч
		/// </summary>
		[AdvancedInspector.Descriptor(" ��Ч", " ��Ч")]
		AMIT_INVALID = 0,
		/// <summary>
		///  ����
		/// </summary>
		[AdvancedInspector.Descriptor(" ����", " ����")]
		AMIT_WEAPON = 1,
		/// <summary>
		///  ����
		/// </summary>
		[AdvancedInspector.Descriptor(" ����", " ����")]
		AMIT_ARMOR = 2,
		/// <summary>
		///  ����
		/// </summary>
		[AdvancedInspector.Descriptor(" ����", " ����")]
		AMIT_JEWELRY = 3,
		/// <summary>
		///  ����Ʒ
		/// </summary>
		[AdvancedInspector.Descriptor(" ����Ʒ", " ����Ʒ")]
		AMIT_COST = 4,
		/// <summary>
		///  ����
		/// </summary>
		[AdvancedInspector.Descriptor(" ����", " ����")]
		AMIT_MATERIAL = 5,
		/// <summary>
		///  ����
		/// </summary>
		[AdvancedInspector.Descriptor(" ����", " ����")]
		AMIT_OTHER = 6,
		/// <summary>
		///  ����
		/// </summary>
		[AdvancedInspector.Descriptor(" ����", " ����")]
		AMIT_NUM = 7,
	}

	/// <summary>
	///  ������ˢ��ԭ��
	/// </summary>
	[AdvancedInspector.Descriptor(" ������ˢ��ԭ��", " ������ˢ��ԭ��")]
	public enum AuctionRefreshReason
	{
		/// <summary>
		///  ����
		/// </summary>
		[AdvancedInspector.Descriptor(" ����", " ����")]
		SRR_BUY = 0,
		/// <summary>
		///  �ϼ�
		/// </summary>
		[AdvancedInspector.Descriptor(" �ϼ�", " �ϼ�")]
		SRR_SELL = 1,
		/// <summary>
		///  �¼�
		/// </summary>
		[AdvancedInspector.Descriptor(" �¼�", " �¼�")]
		SRR_CANCEL = 2,
		/// <summary>
		///  ����
		/// </summary>
		[AdvancedInspector.Descriptor(" ����", " ����")]
		SRR_RUSY_BUY = 3,
		/// <summary>
		///  ɨ��
		/// </summary>
		[AdvancedInspector.Descriptor(" ɨ��", " ɨ��")]
		SRR_SYS_RECOVERY = 4,
	}

	/// <summary>
	///  ��Ʒ����״̬
	/// </summary>
	[AdvancedInspector.Descriptor(" ��Ʒ����״̬", " ��Ʒ����״̬")]
	public enum AuctionGoodState
	{
		/// <summary>
		///  ��Ч
		/// </summary>
		[AdvancedInspector.Descriptor(" ��Ч", " ��Ч")]
		AGS_INVALID = 0,
		/// <summary>
		///  ��ʾ
		/// </summary>
		[AdvancedInspector.Descriptor(" ��ʾ", " ��ʾ")]
		AGS_PUBLIC = 1,
		/// <summary>
		///  �ϼ�
		/// </summary>
		[AdvancedInspector.Descriptor(" �ϼ�", " �ϼ�")]
		AGS_ONSALE = 2,
	}

	/// <summary>
	/// �����й�ע����
	/// </summary>
	[AdvancedInspector.Descriptor("�����й�ע����", "�����й�ע����")]
	public enum AuctionAttentOpType
	{
		/// <summary>
		/// ��ע
		/// </summary>
		[AdvancedInspector.Descriptor("��ע", "��ע")]
		ATOT_ATTENT = 1,
		/// <summary>
		/// ȡ����ע
		/// </summary>
		[AdvancedInspector.Descriptor("ȡ����ע", "ȡ����ע")]
		ATOT_CANCEL = 2,
	}

	/// <summary>
	/// �����й�ע����
	/// </summary>
	[AdvancedInspector.Descriptor("�����й�ע����", "�����й�ע����")]
	public enum AuctionAttentType
	{
		/// <summary>
		/// �ǹ�ע
		/// </summary>
		[AdvancedInspector.Descriptor("�ǹ�ע", "�ǹ�ע")]
		ATT_NOTATTENT = 0,
		/// <summary>
		/// ��ע
		/// </summary>
		[AdvancedInspector.Descriptor("��ע", "��ע")]
		ATT_ATTENT = 1,
	}

}
