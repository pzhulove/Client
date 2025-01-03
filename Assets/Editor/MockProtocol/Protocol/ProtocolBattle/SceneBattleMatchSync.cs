using System;
using System.Text;

namespace Mock.Protocol
{

	public class SceneBattleMatchSync : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 508906;
		public UInt32 Sequence;

		public UInt32 battleID;

		public UInt32 suvivalNum;

		public PlayerSubject[] players = null;

		public UInt32 sceneNodeID;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, battleID);
			BaseDLL.encode_uint32(buffer, ref pos_, suvivalNum);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)players.Length);
			for(int i = 0; i < players.Length; i++)
			{
				players[i].encode(buffer, ref pos_);
			}
			BaseDLL.encode_uint32(buffer, ref pos_, sceneNodeID);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref battleID);
			BaseDLL.decode_uint32(buffer, ref pos_, ref suvivalNum);
			UInt16 playersCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref playersCnt);
			players = new PlayerSubject[playersCnt];
			for(int i = 0; i < players.Length; i++)
			{
				players[i] = new PlayerSubject();
				players[i].decode(buffer, ref pos_);
			}
			BaseDLL.decode_uint32(buffer, ref pos_, ref sceneNodeID);
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
