using System;
using System.Text;

namespace Mock.Protocol
{

	public class TeamCopyInviteListRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 1100053;
		public UInt32 Sequence;

		public TCInviteInfo[] inviteList = null;

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
				inviteList[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 inviteListCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref inviteListCnt);
			inviteList = new TCInviteInfo[inviteListCnt];
			for(int i = 0; i < inviteList.Length; i++)
			{
				inviteList[i] = new TCInviteInfo();
				inviteList[i].decode(buffer, ref pos_);
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
