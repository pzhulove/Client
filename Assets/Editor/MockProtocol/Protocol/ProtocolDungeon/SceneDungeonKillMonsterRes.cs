using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 玩家杀死怪物返回
	/// </summary>
	[AdvancedInspector.Descriptor("玩家杀死怪物返回", "玩家杀死怪物返回")]
	public class SceneDungeonKillMonsterRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 506820;
		public UInt32 Sequence;

		public UInt32[] monsterIds = new UInt32[0];

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)monsterIds.Length);
			for(int i = 0; i < monsterIds.Length; i++)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, monsterIds[i]);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 monsterIdsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref monsterIdsCnt);
			monsterIds = new UInt32[monsterIdsCnt];
			for(int i = 0; i < monsterIds.Length; i++)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref monsterIds[i]);
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
