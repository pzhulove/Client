using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// svr->clt更新聊天玩家的在线信息
	/// </summary>
	[AdvancedInspector.Descriptor("svr->clt更新聊天玩家的在线信息", "svr->clt更新聊天玩家的在线信息")]
	public class WorldUpdatePlayerOnlineRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601715;
		public UInt32 Sequence;

		public PlayerOnline[] playerStates = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)playerStates.Length);
			for(int i = 0; i < playerStates.Length; i++)
			{
				playerStates[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 playerStatesCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref playerStatesCnt);
			playerStates = new PlayerOnline[playerStatesCnt];
			for(int i = 0; i < playerStates.Length; i++)
			{
				playerStates[i] = new PlayerOnline();
				playerStates[i].decode(buffer, ref pos_);
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
