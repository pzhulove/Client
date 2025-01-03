using System;
using System.Text;

namespace Mock.Protocol
{

	public class SceneDungeonStartReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 506805;
		public UInt32 Sequence;

		public UInt32 dungeonId;
		/// <summary>
		///  要使用的药水
		/// </summary>
		[AdvancedInspector.Descriptor(" 要使用的药水", " 要使用的药水")]
		public UInt32[] buffDrugs = new UInt32[0];
		/// <summary>
		///  是否重新开始
		/// </summary>
		[AdvancedInspector.Descriptor(" 是否重新开始", " 是否重新开始")]
		public byte isRestart;
		/// <summary>
		///  如果是城镇怪物、填入城镇怪物ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 如果是城镇怪物、填入城镇怪物ID", " 如果是城镇怪物、填入城镇怪物ID")]
		public UInt64 cityMonsterId;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, dungeonId);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)buffDrugs.Length);
			for(int i = 0; i < buffDrugs.Length; i++)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, buffDrugs[i]);
			}
			BaseDLL.encode_int8(buffer, ref pos_, isRestart);
			BaseDLL.encode_uint64(buffer, ref pos_, cityMonsterId);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref dungeonId);
			UInt16 buffDrugsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref buffDrugsCnt);
			buffDrugs = new UInt32[buffDrugsCnt];
			for(int i = 0; i < buffDrugs.Length; i++)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref buffDrugs[i]);
			}
			BaseDLL.decode_int8(buffer, ref pos_, ref isRestart);
			BaseDLL.decode_uint64(buffer, ref pos_, ref cityMonsterId);
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
