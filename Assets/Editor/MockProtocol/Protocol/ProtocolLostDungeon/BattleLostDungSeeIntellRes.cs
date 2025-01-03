using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// battleScene->client  查看玩家情报返回
	/// </summary>
	[AdvancedInspector.Descriptor("battleScene->client  查看玩家情报返回", "battleScene->client  查看玩家情报返回")]
	public class BattleLostDungSeeIntellRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 510031;
		public UInt32 Sequence;

		public UInt32 code;

		public UInt64 playerId;

		public ItemReward[] artifacts = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, code);
			BaseDLL.encode_uint64(buffer, ref pos_, playerId);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)artifacts.Length);
			for(int i = 0; i < artifacts.Length; i++)
			{
				artifacts[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			BaseDLL.decode_uint64(buffer, ref pos_, ref playerId);
			UInt16 artifactsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref artifactsCnt);
			artifacts = new ItemReward[artifactsCnt];
			for(int i = 0; i < artifacts.Length; i++)
			{
				artifacts[i] = new ItemReward();
				artifacts[i].decode(buffer, ref pos_);
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
