using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  返回活动怪物信息
	/// </summary>
	[AdvancedInspector.Descriptor(" 返回活动怪物信息", " 返回活动怪物信息")]
	public class WorldActivityMonsterRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 607405;
		public UInt32 Sequence;

		public UInt32 activityId;

		public ActivityMonsterInfo[] monsters = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, activityId);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)monsters.Length);
			for(int i = 0; i < monsters.Length; i++)
			{
				monsters[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref activityId);
			UInt16 monstersCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref monstersCnt);
			monsters = new ActivityMonsterInfo[monstersCnt];
			for(int i = 0; i < monsters.Length; i++)
			{
				monsters[i] = new ActivityMonsterInfo();
				monsters[i].decode(buffer, ref pos_);
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
