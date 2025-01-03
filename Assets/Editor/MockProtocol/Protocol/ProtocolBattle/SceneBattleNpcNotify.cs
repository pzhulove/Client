using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  刷出NPC
	/// </summary>
	[AdvancedInspector.Descriptor(" 刷出NPC", " 刷出NPC")]
	public class SceneBattleNpcNotify : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 508929;
		public UInt32 Sequence;

		public BattleNpc[] battleNpcList = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)battleNpcList.Length);
			for(int i = 0; i < battleNpcList.Length; i++)
			{
				battleNpcList[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 battleNpcListCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref battleNpcListCnt);
			battleNpcList = new BattleNpc[battleNpcListCnt];
			for(int i = 0; i < battleNpcList.Length; i++)
			{
				battleNpcList[i] = new BattleNpc();
				battleNpcList[i].decode(buffer, ref pos_);
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
