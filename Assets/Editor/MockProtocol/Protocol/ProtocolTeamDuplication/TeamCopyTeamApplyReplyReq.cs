using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  申请处理
	/// </summary>
	[AdvancedInspector.Descriptor(" 申请处理", " 申请处理")]
	public class TeamCopyTeamApplyReplyReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 1100025;
		public UInt32 Sequence;
		/// <summary>
		///  非0同意，0拒绝
		/// </summary>
		[AdvancedInspector.Descriptor(" 非0同意，0拒绝", " 非0同意，0拒绝")]
		public UInt32 isAgree;
		/// <summary>
		///  玩家列表
		/// </summary>
		[AdvancedInspector.Descriptor(" 玩家列表", " 玩家列表")]
		public UInt64[] playerIds = new UInt64[0];

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, isAgree);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)playerIds.Length);
			for(int i = 0; i < playerIds.Length; i++)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, playerIds[i]);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref isAgree);
			UInt16 playerIdsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref playerIdsCnt);
			playerIds = new UInt64[playerIdsCnt];
			for(int i = 0; i < playerIds.Length; i++)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref playerIds[i]);
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
