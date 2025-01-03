using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  公会地下城伤害排行返回
	/// </summary>
	[AdvancedInspector.Descriptor(" 公会地下城伤害排行返回", " 公会地下城伤害排行返回")]
	public class WorldGuildDungeonDamageRankRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 608504;
		public UInt32 Sequence;
		/// <summary>
		///  伤害列表
		/// </summary>
		[AdvancedInspector.Descriptor(" 伤害列表", " 伤害列表")]
		public GuildDungeonDamage[] damageVec = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)damageVec.Length);
			for(int i = 0; i < damageVec.Length; i++)
			{
				damageVec[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 damageVecCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref damageVecCnt);
			damageVec = new GuildDungeonDamage[damageVecCnt];
			for(int i = 0; i < damageVec.Length; i++)
			{
				damageVec[i] = new GuildDungeonDamage();
				damageVec[i].decode(buffer, ref pos_);
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
