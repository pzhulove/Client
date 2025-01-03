using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  小队数据
	/// </summary>
	[AdvancedInspector.Descriptor(" 小队数据", " 小队数据")]
	public class SquadData : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  小队ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 小队ID", " 小队ID")]
		public UInt32 squadId;
		/// <summary>
		///  小队状态
		/// </summary>
		[AdvancedInspector.Descriptor(" 小队状态", " 小队状态")]
		public UInt32 squadStatus;
		/// <summary>
		///  成员列表
		/// </summary>
		[AdvancedInspector.Descriptor(" 成员列表", " 成员列表")]
		public TeamCopyMember[] teamMemberList = null;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, squadId);
			BaseDLL.encode_uint32(buffer, ref pos_, squadStatus);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)teamMemberList.Length);
			for(int i = 0; i < teamMemberList.Length; i++)
			{
				teamMemberList[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref squadId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref squadStatus);
			UInt16 teamMemberListCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref teamMemberListCnt);
			teamMemberList = new TeamCopyMember[teamMemberListCnt];
			for(int i = 0; i < teamMemberList.Length; i++)
			{
				teamMemberList[i] = new TeamCopyMember();
				teamMemberList[i].decode(buffer, ref pos_);
			}
		}


		#endregion

	}

}
