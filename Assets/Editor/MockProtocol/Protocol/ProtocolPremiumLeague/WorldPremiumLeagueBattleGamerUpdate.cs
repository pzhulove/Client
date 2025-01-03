using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  更新淘汰赛玩家信息
	/// </summary>
	[AdvancedInspector.Descriptor(" 更新淘汰赛玩家信息", " 更新淘汰赛玩家信息")]
	public class WorldPremiumLeagueBattleGamerUpdate : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 607707;
		public UInt32 Sequence;

		public UInt64 roleId;

		public UInt32 winNum;

		public byte isLose;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, roleId);
			BaseDLL.encode_uint32(buffer, ref pos_, winNum);
			BaseDLL.encode_int8(buffer, ref pos_, isLose);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref winNum);
			BaseDLL.decode_int8(buffer, ref pos_, ref isLose);
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
