using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  团队列表返回
	/// </summary>
	[AdvancedInspector.Descriptor(" 团队列表返回", " 团队列表返回")]
	public class TeamCopyTeamListRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 1100008;
		public UInt32 Sequence;
		/// <summary>
		///  当前页
		/// </summary>
		[AdvancedInspector.Descriptor(" 当前页", " 当前页")]
		public UInt32 curPage;
		/// <summary>
		///  总页数
		/// </summary>
		[AdvancedInspector.Descriptor(" 总页数", " 总页数")]
		public UInt32 totalPageNum;
		/// <summary>
		///  团队列表
		/// </summary>
		[AdvancedInspector.Descriptor(" 团队列表", " 团队列表")]
		public TeamCopyTeamProperty[] teamList = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, curPage);
			BaseDLL.encode_uint32(buffer, ref pos_, totalPageNum);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)teamList.Length);
			for(int i = 0; i < teamList.Length; i++)
			{
				teamList[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref curPage);
			BaseDLL.decode_uint32(buffer, ref pos_, ref totalPageNum);
			UInt16 teamListCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref teamListCnt);
			teamList = new TeamCopyTeamProperty[teamListCnt];
			for(int i = 0; i < teamList.Length; i++)
			{
				teamList[i] = new TeamCopyTeamProperty();
				teamList[i].decode(buffer, ref pos_);
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
