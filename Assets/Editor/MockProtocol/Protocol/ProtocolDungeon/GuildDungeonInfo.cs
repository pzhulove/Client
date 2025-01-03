using System;
using System.Text;

namespace Mock.Protocol
{

	public class GuildDungeonInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  boss剩余血量
		/// </summary>
		[AdvancedInspector.Descriptor(" boss剩余血量", " boss剩余血量")]
		public UInt64 bossOddBlood;
		/// <summary>
		///  boss总血量
		/// </summary>
		[AdvancedInspector.Descriptor(" boss总血量", " boss总血量")]
		public UInt64 bossTotalBlood;
		/// <summary>
		///  加成buff
		/// </summary>
		[AdvancedInspector.Descriptor(" 加成buff", " 加成buff")]
		public DungeonBuff[] buffVec = null;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, bossOddBlood);
			BaseDLL.encode_uint64(buffer, ref pos_, bossTotalBlood);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)buffVec.Length);
			for(int i = 0; i < buffVec.Length; i++)
			{
				buffVec[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref bossOddBlood);
			BaseDLL.decode_uint64(buffer, ref pos_, ref bossTotalBlood);
			UInt16 buffVecCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref buffVecCnt);
			buffVec = new DungeonBuff[buffVecCnt];
			for(int i = 0; i < buffVec.Length; i++)
			{
				buffVec[i] = new DungeonBuff();
				buffVec[i].decode(buffer, ref pos_);
			}
		}


		#endregion

	}

}
