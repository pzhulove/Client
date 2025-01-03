using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  初始化淘汰赛玩家列表
	/// </summary>
	[AdvancedInspector.Descriptor(" 初始化淘汰赛玩家列表", " 初始化淘汰赛玩家列表")]
	public class WorldPremiumLeagueBattleGamerInit : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 607706;
		public UInt32 Sequence;

		public PremiumLeagueBattleGamer[] gamers = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)gamers.Length);
			for(int i = 0; i < gamers.Length; i++)
			{
				gamers[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 gamersCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref gamersCnt);
			gamers = new PremiumLeagueBattleGamer[gamersCnt];
			for(int i = 0; i < gamers.Length; i++)
			{
				gamers[i] = new PremiumLeagueBattleGamer();
				gamers[i].decode(buffer, ref pos_);
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
