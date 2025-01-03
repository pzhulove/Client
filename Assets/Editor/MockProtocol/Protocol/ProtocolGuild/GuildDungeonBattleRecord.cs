using System;
using System.Text;

namespace Mock.Protocol
{

	public class GuildDungeonBattleRecord : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  地下城id
		/// </summary>
		[AdvancedInspector.Descriptor(" 地下城id", " 地下城id")]
		public UInt32 dungeonId;
		/// <summary>
		///  战斗次数
		/// </summary>
		[AdvancedInspector.Descriptor(" 战斗次数", " 战斗次数")]
		public UInt32 battleCnt;
		/// <summary>
		///  剩余血量
		/// </summary>
		[AdvancedInspector.Descriptor(" 剩余血量", " 剩余血量")]
		public UInt64 oddBlood;
		/// <summary>
		///  buff列表
		/// </summary>
		[AdvancedInspector.Descriptor(" buff列表", " buff列表")]
		public GuildDungeonBuff[] buffVec = null;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, dungeonId);
			BaseDLL.encode_uint32(buffer, ref pos_, battleCnt);
			BaseDLL.encode_uint64(buffer, ref pos_, oddBlood);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)buffVec.Length);
			for(int i = 0; i < buffVec.Length; i++)
			{
				buffVec[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref dungeonId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref battleCnt);
			BaseDLL.decode_uint64(buffer, ref pos_, ref oddBlood);
			UInt16 buffVecCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref buffVecCnt);
			buffVec = new GuildDungeonBuff[buffVecCnt];
			for(int i = 0; i < buffVec.Length; i++)
			{
				buffVec[i] = new GuildDungeonBuff();
				buffVec[i].decode(buffer, ref pos_);
			}
		}


		#endregion

	}

}
