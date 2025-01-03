using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  周签到宝箱数据
	/// </summary>
	[AdvancedInspector.Descriptor(" 周签到宝箱数据", " 周签到宝箱数据")]
	public class WeekSignBox : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  活动Id
		/// </summary>
		[AdvancedInspector.Descriptor(" 活动Id", " 活动Id")]
		public UInt32 opActId;
		/// <summary>
		///  奖励列表
		/// </summary>
		[AdvancedInspector.Descriptor(" 奖励列表", " 奖励列表")]
		public ItemReward[] itemVec = null;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, opActId);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)itemVec.Length);
			for(int i = 0; i < itemVec.Length; i++)
			{
				itemVec[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref opActId);
			UInt16 itemVecCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref itemVecCnt);
			itemVec = new ItemReward[itemVecCnt];
			for(int i = 0; i < itemVec.Length; i++)
			{
				itemVec[i] = new ItemReward();
				itemVec[i].decode(buffer, ref pos_);
			}
		}


		#endregion

	}

}
