using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  邀请选择
	/// </summary>
	[AdvancedInspector.Descriptor(" 邀请选择", " 邀请选择")]
	public class TeamCopyInviteChoiceReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 1100054;
		public UInt32 Sequence;
		/// <summary>
		///  0拒绝，非0同意
		/// </summary>
		[AdvancedInspector.Descriptor(" 0拒绝，非0同意", " 0拒绝，非0同意")]
		public UInt32 isAgree;
		/// <summary>
		///  队伍Id
		/// </summary>
		[AdvancedInspector.Descriptor(" 队伍Id", " 队伍Id")]
		public UInt32[] teamId = new UInt32[0];

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, isAgree);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)teamId.Length);
			for(int i = 0; i < teamId.Length; i++)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, teamId[i]);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref isAgree);
			UInt16 teamIdCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref teamIdCnt);
			teamId = new UInt32[teamIdCnt];
			for(int i = 0; i < teamId.Length; i++)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref teamId[i]);
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
