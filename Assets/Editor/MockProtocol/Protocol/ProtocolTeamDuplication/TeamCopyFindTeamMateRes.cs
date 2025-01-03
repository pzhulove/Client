using System;
using System.Text;

namespace Mock.Protocol
{

	public class TeamCopyFindTeamMateRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 1100030;
		public UInt32 Sequence;
		/// <summary>
		///  玩家
		/// </summary>
		[AdvancedInspector.Descriptor(" 玩家", " 玩家")]
		public TeamCopyApplyProperty[] playerList = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)playerList.Length);
			for(int i = 0; i < playerList.Length; i++)
			{
				playerList[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 playerListCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref playerListCnt);
			playerList = new TeamCopyApplyProperty[playerListCnt];
			for(int i = 0; i < playerList.Length; i++)
			{
				playerList[i] = new TeamCopyApplyProperty();
				playerList[i].decode(buffer, ref pos_);
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
