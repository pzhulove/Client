using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  投票完成
	/// </summary>
	[AdvancedInspector.Descriptor(" 投票完成", " 投票完成")]
	public class TeamCopyVoteFinish : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 1100018;
		public UInt32 Sequence;
		/// <summary>
		///  结果(0代表成功，非0代表失败)
		/// </summary>
		[AdvancedInspector.Descriptor(" 结果(0代表成功，非0代表失败)", " 结果(0代表成功，非0代表失败)")]
		public UInt32 result;
		/// <summary>
		///  未投票玩家
		/// </summary>
		[AdvancedInspector.Descriptor(" 未投票玩家", " 未投票玩家")]
		public string[] notVotePlayer = new string[0];

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, result);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)notVotePlayer.Length);
			for(int i = 0; i < notVotePlayer.Length; i++)
			{
				byte[] notVotePlayerBytes = StringHelper.StringToUTF8Bytes(notVotePlayer[i]);
				BaseDLL.encode_string(buffer, ref pos_, notVotePlayerBytes, (UInt16)(buffer.Length - pos_));
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			UInt16 notVotePlayerCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref notVotePlayerCnt);
			notVotePlayer = new string[notVotePlayerCnt];
			for(int i = 0; i < notVotePlayer.Length; i++)
			{
				UInt16 notVotePlayerLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref notVotePlayerLen);
				byte[] notVotePlayerBytes = new byte[notVotePlayerLen];
				for(int j = 0; j < notVotePlayerLen; j++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref notVotePlayerBytes[j]);
				}
				notVotePlayer[i] = StringHelper.BytesToString(notVotePlayerBytes);
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
