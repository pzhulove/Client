using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  邀请
	/// </summary>
	[AdvancedInspector.Descriptor(" 邀请", " 邀请")]
	public class TeamCopyInvitePlayer : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 1100031;
		public UInt32 Sequence;
		/// <summary>
		///  邀请列表
		/// </summary>
		[AdvancedInspector.Descriptor(" 邀请列表", " 邀请列表")]
		public UInt64[] inviteList = new UInt64[0];

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)inviteList.Length);
			for(int i = 0; i < inviteList.Length; i++)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, inviteList[i]);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 inviteListCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref inviteListCnt);
			inviteList = new UInt64[inviteListCnt];
			for(int i = 0; i < inviteList.Length; i++)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref inviteList[i]);
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
