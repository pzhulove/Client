using System;
using System.Text;

namespace Mock.Protocol
{

	public class TeamCopyTeamDetailRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 1100044;
		public UInt32 Sequence;
		/// <summary>
		///  队伍ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 队伍ID", " 队伍ID")]
		public UInt32 teamId;
		/// <summary>
		///  队伍模式
		/// </summary>
		[AdvancedInspector.Descriptor(" 队伍模式", " 队伍模式")]
		public UInt32 teamModel;
		/// <summary>
		///  团队名字
		/// </summary>
		[AdvancedInspector.Descriptor(" 团队名字", " 团队名字")]
		public string teamName;
		/// <summary>
		///  佣金总数
		/// </summary>
		[AdvancedInspector.Descriptor(" 佣金总数", " 佣金总数")]
		public UInt32 totalCommission;
		/// <summary>
		///  分成佣金
		/// </summary>
		[AdvancedInspector.Descriptor(" 分成佣金", " 分成佣金")]
		public UInt32 bonusCommission;
		/// <summary>
		///  小队列表
		/// </summary>
		[AdvancedInspector.Descriptor(" 小队列表", " 小队列表")]
		public SquadData[] squadList = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, teamId);
			BaseDLL.encode_uint32(buffer, ref pos_, teamModel);
			byte[] teamNameBytes = StringHelper.StringToUTF8Bytes(teamName);
			BaseDLL.encode_string(buffer, ref pos_, teamNameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_uint32(buffer, ref pos_, totalCommission);
			BaseDLL.encode_uint32(buffer, ref pos_, bonusCommission);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)squadList.Length);
			for(int i = 0; i < squadList.Length; i++)
			{
				squadList[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref teamId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref teamModel);
			UInt16 teamNameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref teamNameLen);
			byte[] teamNameBytes = new byte[teamNameLen];
			for(int i = 0; i < teamNameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref teamNameBytes[i]);
			}
			teamName = StringHelper.BytesToString(teamNameBytes);
			BaseDLL.decode_uint32(buffer, ref pos_, ref totalCommission);
			BaseDLL.decode_uint32(buffer, ref pos_, ref bonusCommission);
			UInt16 squadListCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref squadListCnt);
			squadList = new SquadData[squadListCnt];
			for(int i = 0; i < squadList.Length; i++)
			{
				squadList[i] = new SquadData();
				squadList[i].decode(buffer, ref pos_);
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
