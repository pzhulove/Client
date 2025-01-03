using System;
using System.Text;

namespace Mock.Protocol
{

	public class ZjslDungeonInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  BOSS1的ID
		/// </summary>
		[AdvancedInspector.Descriptor(" BOSS1的ID", " BOSS1的ID")]
		public UInt32 boss1ID;
		/// <summary>
		///  BOSS2的ID
		/// </summary>
		[AdvancedInspector.Descriptor(" BOSS2的ID", " BOSS2的ID")]
		public UInt32 boss2ID;
		/// <summary>
		///  BOSS1的剩余血量
		/// </summary>
		[AdvancedInspector.Descriptor(" BOSS1的剩余血量", " BOSS1的剩余血量")]
		public UInt32 boss1RemainHp;
		/// <summary>
		///  BOSS2的剩
		/// </summary>
		[AdvancedInspector.Descriptor(" BOSS2的剩", " BOSS2的剩")]
		public UInt32 boss2RemainHp;
		/// <summary>
		///  加成buff
		/// </summary>
		[AdvancedInspector.Descriptor(" 加成buff", " 加成buff")]
		public ZjslDungeonBuff[] buffVec = null;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, boss1ID);
			BaseDLL.encode_uint32(buffer, ref pos_, boss2ID);
			BaseDLL.encode_uint32(buffer, ref pos_, boss1RemainHp);
			BaseDLL.encode_uint32(buffer, ref pos_, boss2RemainHp);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)buffVec.Length);
			for(int i = 0; i < buffVec.Length; i++)
			{
				buffVec[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref boss1ID);
			BaseDLL.decode_uint32(buffer, ref pos_, ref boss2ID);
			BaseDLL.decode_uint32(buffer, ref pos_, ref boss1RemainHp);
			BaseDLL.decode_uint32(buffer, ref pos_, ref boss2RemainHp);
			UInt16 buffVecCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref buffVecCnt);
			buffVec = new ZjslDungeonBuff[buffVecCnt];
			for(int i = 0; i < buffVec.Length; i++)
			{
				buffVec[i] = new ZjslDungeonBuff();
				buffVec[i].decode(buffer, ref pos_);
			}
		}


		#endregion

	}

}
