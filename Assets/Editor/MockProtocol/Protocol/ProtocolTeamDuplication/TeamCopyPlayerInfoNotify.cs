using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  玩家的团本数据
	/// </summary>
	[AdvancedInspector.Descriptor(" 玩家的团本数据", " 玩家的团本数据")]
	public class TeamCopyPlayerInfoNotify : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 1100059;
		public UInt32 Sequence;
		/// <summary>
		///  普通日次数
		/// </summary>
		[AdvancedInspector.Descriptor(" 普通日次数", " 普通日次数")]
		public UInt32 dayNum;
		/// <summary>
		///  普通日总次数
		/// </summary>
		[AdvancedInspector.Descriptor(" 普通日总次数", " 普通日总次数")]
		public UInt32 dayTotalNum;
		/// <summary>
		///  普通周次数
		/// </summary>
		[AdvancedInspector.Descriptor(" 普通周次数", " 普通周次数")]
		public UInt32 weekNum;
		/// <summary>
		///  普通周总次数
		/// </summary>
		[AdvancedInspector.Descriptor(" 普通周总次数", " 普通周总次数")]
		public UInt32 weekTotalNum;
		/// <summary>
		///  噩梦周次数
		/// </summary>
		[AdvancedInspector.Descriptor(" 噩梦周次数", " 噩梦周次数")]
		public UInt32 diffWeekNum;
		/// <summary>
		///  噩梦周总次数
		/// </summary>
		[AdvancedInspector.Descriptor(" 噩梦周总次数", " 噩梦周总次数")]
		public UInt32 diffWeekTotalNum;
		/// <summary>
		///  是否可以创建金团
		/// </summary>
		[AdvancedInspector.Descriptor(" 是否可以创建金团", " 是否可以创建金团")]
		public UInt32 isCreateGold;
		/// <summary>
		///  日免费退出次数
		/// </summary>
		[AdvancedInspector.Descriptor(" 日免费退出次数", " 日免费退出次数")]
		public UInt32 dayFreeQuitNum;
		/// <summary>
		///  周免费退出次数
		/// </summary>
		[AdvancedInspector.Descriptor(" 周免费退出次数", " 周免费退出次数")]
		public UInt32 weekFreeQuitNum;
		/// <summary>
		///  门票是否足够
		/// </summary>
		[AdvancedInspector.Descriptor(" 门票是否足够", " 门票是否足够")]
		public UInt32 ticketIsEnough;
		/// <summary>
		///  普通难度通关次数
		/// </summary>
		[AdvancedInspector.Descriptor(" 普通难度通关次数", " 普通难度通关次数")]
		public UInt32 commonGradePassNum;
		/// <summary>
		///  解锁噩梦需要的普通难度次数
		/// </summary>
		[AdvancedInspector.Descriptor(" 解锁噩梦需要的普通难度次数", " 解锁噩梦需要的普通难度次数")]
		public UInt32 unlockDiffGradeCommonNum;
		/// <summary>
		///  已开的难度
		/// </summary>
		[AdvancedInspector.Descriptor(" 已开的难度", " 已开的难度")]
		public UInt32[] yetOpenGradeList = new UInt32[0];

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, dayNum);
			BaseDLL.encode_uint32(buffer, ref pos_, dayTotalNum);
			BaseDLL.encode_uint32(buffer, ref pos_, weekNum);
			BaseDLL.encode_uint32(buffer, ref pos_, weekTotalNum);
			BaseDLL.encode_uint32(buffer, ref pos_, diffWeekNum);
			BaseDLL.encode_uint32(buffer, ref pos_, diffWeekTotalNum);
			BaseDLL.encode_uint32(buffer, ref pos_, isCreateGold);
			BaseDLL.encode_uint32(buffer, ref pos_, dayFreeQuitNum);
			BaseDLL.encode_uint32(buffer, ref pos_, weekFreeQuitNum);
			BaseDLL.encode_uint32(buffer, ref pos_, ticketIsEnough);
			BaseDLL.encode_uint32(buffer, ref pos_, commonGradePassNum);
			BaseDLL.encode_uint32(buffer, ref pos_, unlockDiffGradeCommonNum);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)yetOpenGradeList.Length);
			for(int i = 0; i < yetOpenGradeList.Length; i++)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, yetOpenGradeList[i]);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref dayNum);
			BaseDLL.decode_uint32(buffer, ref pos_, ref dayTotalNum);
			BaseDLL.decode_uint32(buffer, ref pos_, ref weekNum);
			BaseDLL.decode_uint32(buffer, ref pos_, ref weekTotalNum);
			BaseDLL.decode_uint32(buffer, ref pos_, ref diffWeekNum);
			BaseDLL.decode_uint32(buffer, ref pos_, ref diffWeekTotalNum);
			BaseDLL.decode_uint32(buffer, ref pos_, ref isCreateGold);
			BaseDLL.decode_uint32(buffer, ref pos_, ref dayFreeQuitNum);
			BaseDLL.decode_uint32(buffer, ref pos_, ref weekFreeQuitNum);
			BaseDLL.decode_uint32(buffer, ref pos_, ref ticketIsEnough);
			BaseDLL.decode_uint32(buffer, ref pos_, ref commonGradePassNum);
			BaseDLL.decode_uint32(buffer, ref pos_, ref unlockDiffGradeCommonNum);
			UInt16 yetOpenGradeListCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref yetOpenGradeListCnt);
			yetOpenGradeList = new UInt32[yetOpenGradeListCnt];
			for(int i = 0; i < yetOpenGradeList.Length; i++)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref yetOpenGradeList[i]);
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
