using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  周签到数据下发
	/// </summary>
	[AdvancedInspector.Descriptor(" 周签到数据下发", " 周签到数据下发")]
	public class SceneWeekSignNotify : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 507406;
		public UInt32 Sequence;
		/// <summary>
		///  活动类型
		/// </summary>
		[AdvancedInspector.Descriptor(" 活动类型", " 活动类型")]
		public UInt32 opActType;
		/// <summary>
		///  已签到周数
		/// </summary>
		[AdvancedInspector.Descriptor(" 已签到周数", " 已签到周数")]
		public UInt32 signWeekSum;
		/// <summary>
		///  已经领取奖励的周
		/// </summary>
		[AdvancedInspector.Descriptor(" 已经领取奖励的周", " 已经领取奖励的周")]
		public UInt32[] yetWeek = new UInt32[0];

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, opActType);
			BaseDLL.encode_uint32(buffer, ref pos_, signWeekSum);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)yetWeek.Length);
			for(int i = 0; i < yetWeek.Length; i++)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, yetWeek[i]);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref opActType);
			BaseDLL.decode_uint32(buffer, ref pos_, ref signWeekSum);
			UInt16 yetWeekCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref yetWeekCnt);
			yetWeek = new UInt32[yetWeekCnt];
			for(int i = 0; i < yetWeek.Length; i++)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref yetWeek[i]);
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
