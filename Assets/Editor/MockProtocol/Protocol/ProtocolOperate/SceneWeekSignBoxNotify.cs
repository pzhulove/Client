using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  周签到宝箱数据下发
	/// </summary>
	[AdvancedInspector.Descriptor(" 周签到宝箱数据下发", " 周签到宝箱数据下发")]
	public class SceneWeekSignBoxNotify : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 507407;
		public UInt32 Sequence;
		/// <summary>
		///  活动类型
		/// </summary>
		[AdvancedInspector.Descriptor(" 活动类型", " 活动类型")]
		public UInt32 opActType;
		/// <summary>
		///  宝箱数据
		/// </summary>
		[AdvancedInspector.Descriptor(" 宝箱数据", " 宝箱数据")]
		public WeekSignBox[] boxData = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, opActType);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)boxData.Length);
			for(int i = 0; i < boxData.Length; i++)
			{
				boxData[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref opActType);
			UInt16 boxDataCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref boxDataCnt);
			boxData = new WeekSignBox[boxDataCnt];
			for(int i = 0; i < boxData.Length; i++)
			{
				boxData[i] = new WeekSignBox();
				boxData[i].decode(buffer, ref pos_);
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
