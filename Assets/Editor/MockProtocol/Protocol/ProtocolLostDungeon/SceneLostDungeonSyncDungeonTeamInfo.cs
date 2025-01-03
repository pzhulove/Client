using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// scene->client 同步地下城组队信息
	/// </summary>
	[AdvancedInspector.Descriptor("scene->client 同步地下城组队信息", "scene->client 同步地下城组队信息")]
	public class SceneLostDungeonSyncDungeonTeamInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 510010;
		public UInt32 Sequence;

		public byte type;
		/// <summary>
		/// 枚举LostDungChangeMode
		/// </summary>
		[AdvancedInspector.Descriptor("枚举LostDungChangeMode", "枚举LostDungChangeMode")]
		public byte challengeMode;

		public LostDungTeamMember[] teamMemebers = null;

		public LostDungTeamMember addMember = null;

		public UInt64 leavePlayerId;

		public LostDungTeamInfo[] teamInfos = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, type);
			BaseDLL.encode_int8(buffer, ref pos_, challengeMode);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)teamMemebers.Length);
			for(int i = 0; i < teamMemebers.Length; i++)
			{
				teamMemebers[i].encode(buffer, ref pos_);
			}
			addMember.encode(buffer, ref pos_);
			BaseDLL.encode_uint64(buffer, ref pos_, leavePlayerId);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)teamInfos.Length);
			for(int i = 0; i < teamInfos.Length; i++)
			{
				teamInfos[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref type);
			BaseDLL.decode_int8(buffer, ref pos_, ref challengeMode);
			UInt16 teamMemebersCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref teamMemebersCnt);
			teamMemebers = new LostDungTeamMember[teamMemebersCnt];
			for(int i = 0; i < teamMemebers.Length; i++)
			{
				teamMemebers[i] = new LostDungTeamMember();
				teamMemebers[i].decode(buffer, ref pos_);
			}
			addMember.decode(buffer, ref pos_);
			BaseDLL.decode_uint64(buffer, ref pos_, ref leavePlayerId);
			UInt16 teamInfosCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref teamInfosCnt);
			teamInfos = new LostDungTeamInfo[teamInfosCnt];
			for(int i = 0; i < teamInfos.Length; i++)
			{
				teamInfos[i] = new LostDungTeamInfo();
				teamInfos[i].decode(buffer, ref pos_);
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
