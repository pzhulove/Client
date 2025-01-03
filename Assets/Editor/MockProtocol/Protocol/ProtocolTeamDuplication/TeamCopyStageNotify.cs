using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  阶段通知
	/// </summary>
	[AdvancedInspector.Descriptor(" 阶段通知", " 阶段通知")]
	public class TeamCopyStageNotify : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 1100019;
		public UInt32 Sequence;
		/// <summary>
		///  阶段id
		/// </summary>
		[AdvancedInspector.Descriptor(" 阶段id", " 阶段id")]
		public UInt32 stageId;
		/// <summary>
		///  游戏结束时间
		/// </summary>
		[AdvancedInspector.Descriptor(" 游戏结束时间", " 游戏结束时间")]
		public UInt32 gameOverTime;
		/// <summary>
		///  小队目标
		/// </summary>
		[AdvancedInspector.Descriptor(" 小队目标", " 小队目标")]
		public TeamCopyTarget squadTarget = null;
		/// <summary>
		///  团队目标
		/// </summary>
		[AdvancedInspector.Descriptor(" 团队目标", " 团队目标")]
		public TeamCopyTarget teamTarget = null;
		/// <summary>
		///  据点列表
		/// </summary>
		[AdvancedInspector.Descriptor(" 据点列表", " 据点列表")]
		public TeamCopyFeild[] feildList = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, stageId);
			BaseDLL.encode_uint32(buffer, ref pos_, gameOverTime);
			squadTarget.encode(buffer, ref pos_);
			teamTarget.encode(buffer, ref pos_);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)feildList.Length);
			for(int i = 0; i < feildList.Length; i++)
			{
				feildList[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref stageId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref gameOverTime);
			squadTarget.decode(buffer, ref pos_);
			teamTarget.decode(buffer, ref pos_);
			UInt16 feildListCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref feildListCnt);
			feildList = new TeamCopyFeild[feildListCnt];
			for(int i = 0; i < feildList.Length; i++)
			{
				feildList[i] = new TeamCopyFeild();
				feildList[i].decode(buffer, ref pos_);
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
