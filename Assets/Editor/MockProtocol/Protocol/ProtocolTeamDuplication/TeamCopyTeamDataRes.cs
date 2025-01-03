using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  团队数据返回
	/// </summary>
	[AdvancedInspector.Descriptor(" 团队数据返回", " 团队数据返回")]
	public class TeamCopyTeamDataRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 1100006;
		public UInt32 Sequence;
		/// <summary>
		///  队伍ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 队伍ID", " 队伍ID")]
		public UInt32 teamId;
		/// <summary>
		///  装备评分
		/// </summary>
		[AdvancedInspector.Descriptor(" 装备评分", " 装备评分")]
		public UInt32 equipScore;
		/// <summary>
		///  状态
		/// </summary>
		[AdvancedInspector.Descriptor(" 状态", " 状态")]
		public UInt32 status;
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
		///  自动同意金主入团(0：不同意，1：同意)
		/// </summary>
		[AdvancedInspector.Descriptor(" 自动同意金主入团(0：不同意，1：同意)", " 自动同意金主入团(0：不同意，1：同意)")]
		public UInt32 autoAgreeGold;
		/// <summary>
		///  团本模式
		/// </summary>
		[AdvancedInspector.Descriptor(" 团本模式", " 团本模式")]
		public UInt32 teamModel;
		/// <summary>
		///  难度
		/// </summary>
		[AdvancedInspector.Descriptor(" 难度", " 难度")]
		public UInt32 teamGrade;
		/// <summary>
		///  语音房间id
		/// </summary>
		[AdvancedInspector.Descriptor(" 语音房间id", " 语音房间id")]
		public string voiceRoomId;
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
			BaseDLL.encode_uint32(buffer, ref pos_, equipScore);
			BaseDLL.encode_uint32(buffer, ref pos_, status);
			byte[] teamNameBytes = StringHelper.StringToUTF8Bytes(teamName);
			BaseDLL.encode_string(buffer, ref pos_, teamNameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_uint32(buffer, ref pos_, totalCommission);
			BaseDLL.encode_uint32(buffer, ref pos_, bonusCommission);
			BaseDLL.encode_uint32(buffer, ref pos_, autoAgreeGold);
			BaseDLL.encode_uint32(buffer, ref pos_, teamModel);
			BaseDLL.encode_uint32(buffer, ref pos_, teamGrade);
			byte[] voiceRoomIdBytes = StringHelper.StringToUTF8Bytes(voiceRoomId);
			BaseDLL.encode_string(buffer, ref pos_, voiceRoomIdBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)squadList.Length);
			for(int i = 0; i < squadList.Length; i++)
			{
				squadList[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref teamId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref equipScore);
			BaseDLL.decode_uint32(buffer, ref pos_, ref status);
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
			BaseDLL.decode_uint32(buffer, ref pos_, ref autoAgreeGold);
			BaseDLL.decode_uint32(buffer, ref pos_, ref teamModel);
			BaseDLL.decode_uint32(buffer, ref pos_, ref teamGrade);
			UInt16 voiceRoomIdLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref voiceRoomIdLen);
			byte[] voiceRoomIdBytes = new byte[voiceRoomIdLen];
			for(int i = 0; i < voiceRoomIdLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref voiceRoomIdBytes[i]);
			}
			voiceRoomId = StringHelper.BytesToString(voiceRoomIdBytes);
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
