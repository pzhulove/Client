using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 积分赛组内战斗记录
	/// </summary>
	[AdvancedInspector.Descriptor("积分赛组内战斗记录", "积分赛组内战斗记录")]
	public class SceneChampionScoreBattleRecordsRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 509830;
		public UInt32 Sequence;

		public ScoreBattleRecord[] recods = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)recods.Length);
			for(int i = 0; i < recods.Length; i++)
			{
				recods[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 recodsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref recodsCnt);
			recods = new ScoreBattleRecord[recodsCnt];
			for(int i = 0; i < recods.Length; i++)
			{
				recods[i] = new ScoreBattleRecord();
				recods[i].decode(buffer, ref pos_);
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
