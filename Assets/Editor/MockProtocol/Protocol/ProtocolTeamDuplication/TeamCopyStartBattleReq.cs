using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  开战请求
	/// </summary>
	[AdvancedInspector.Descriptor(" 开战请求", " 开战请求")]
	public class TeamCopyStartBattleReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 1100013;
		public UInt32 Sequence;

		public UInt32 planModel;

		public TeamCopyBattlePlan[] battlePlanList = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, planModel);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)battlePlanList.Length);
			for(int i = 0; i < battlePlanList.Length; i++)
			{
				battlePlanList[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref planModel);
			UInt16 battlePlanListCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref battlePlanListCnt);
			battlePlanList = new TeamCopyBattlePlan[battlePlanListCnt];
			for(int i = 0; i < battlePlanList.Length; i++)
			{
				battlePlanList[i] = new TeamCopyBattlePlan();
				battlePlanList[i].decode(buffer, ref pos_);
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
