using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// clt->svr更新聊天玩家的在线信息
	/// </summary>
	[AdvancedInspector.Descriptor("clt->svr更新聊天玩家的在线信息", "clt->svr更新聊天玩家的在线信息")]
	public class WorldUpdatePlayerOnlineReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601714;
		public UInt32 Sequence;

		public UInt64[] uids = new UInt64[0];

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)uids.Length);
			for(int i = 0; i < uids.Length; i++)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, uids[i]);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 uidsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref uidsCnt);
			uids = new UInt64[uidsCnt];
			for(int i = 0; i < uids.Length; i++)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref uids[i]);
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
