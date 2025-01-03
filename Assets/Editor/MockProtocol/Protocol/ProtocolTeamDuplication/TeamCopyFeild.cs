using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  据点
	/// </summary>
	[AdvancedInspector.Descriptor(" 据点", " 据点")]
	public class TeamCopyFeild : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  据点id
		/// </summary>
		[AdvancedInspector.Descriptor(" 据点id", " 据点id")]
		public UInt32 feildId;
		/// <summary>
		///  剩余次数
		/// </summary>
		[AdvancedInspector.Descriptor(" 剩余次数", " 剩余次数")]
		public UInt32 oddNum;
		/// <summary>
		///  状态
		/// </summary>
		[AdvancedInspector.Descriptor(" 状态", " 状态")]
		public UInt32 state;
		/// <summary>
		///  重生时间
		/// </summary>
		[AdvancedInspector.Descriptor(" 重生时间", " 重生时间")]
		public UInt32 rebornTime;
		/// <summary>
		///  能量恢复时间点
		/// </summary>
		[AdvancedInspector.Descriptor(" 能量恢复时间点", " 能量恢复时间点")]
		public UInt32 energyReviveTime;
		/// <summary>
		///  攻打小队列表
		/// </summary>
		[AdvancedInspector.Descriptor(" 攻打小队列表", " 攻打小队列表")]
		public UInt32[] attackSquadList = new UInt32[0];

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, feildId);
			BaseDLL.encode_uint32(buffer, ref pos_, oddNum);
			BaseDLL.encode_uint32(buffer, ref pos_, state);
			BaseDLL.encode_uint32(buffer, ref pos_, rebornTime);
			BaseDLL.encode_uint32(buffer, ref pos_, energyReviveTime);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)attackSquadList.Length);
			for(int i = 0; i < attackSquadList.Length; i++)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, attackSquadList[i]);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref feildId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref oddNum);
			BaseDLL.decode_uint32(buffer, ref pos_, ref state);
			BaseDLL.decode_uint32(buffer, ref pos_, ref rebornTime);
			BaseDLL.decode_uint32(buffer, ref pos_, ref energyReviveTime);
			UInt16 attackSquadListCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref attackSquadListCnt);
			attackSquadList = new UInt32[attackSquadListCnt];
			for(int i = 0; i < attackSquadList.Length; i++)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref attackSquadList[i]);
			}
		}


		#endregion

	}

}
