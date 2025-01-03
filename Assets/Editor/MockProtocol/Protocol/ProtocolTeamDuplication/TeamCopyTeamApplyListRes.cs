using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  返回申请列表
	/// </summary>
	[AdvancedInspector.Descriptor(" 返回申请列表", " 返回申请列表")]
	public class TeamCopyTeamApplyListRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 1100024;
		public UInt32 Sequence;
		/// <summary>
		///  申请列表
		/// </summary>
		[AdvancedInspector.Descriptor(" 申请列表", " 申请列表")]
		public TeamCopyApplyProperty[] applyList = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)applyList.Length);
			for(int i = 0; i < applyList.Length; i++)
			{
				applyList[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 applyListCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref applyListCnt);
			applyList = new TeamCopyApplyProperty[applyListCnt];
			for(int i = 0; i < applyList.Length; i++)
			{
				applyList[i] = new TeamCopyApplyProperty();
				applyList[i].decode(buffer, ref pos_);
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
