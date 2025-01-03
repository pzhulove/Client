using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  队伍基础信息
	/// </summary>
	[AdvancedInspector.Descriptor(" 队伍基础信息", " 队伍基础信息")]
	public class TeamBaseInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  队伍编号
		/// </summary>
		[AdvancedInspector.Descriptor(" 队伍编号", " 队伍编号")]
		public UInt32 teamId;
		/// <summary>
		///  队伍目标
		/// </summary>
		[AdvancedInspector.Descriptor(" 队伍目标", " 队伍目标")]
		public UInt32 target;
		/// <summary>
		///  队长信息
		/// </summary>
		[AdvancedInspector.Descriptor(" 队长信息", " 队长信息")]
		public TeammemberBaseInfo masterInfo = null;
		/// <summary>
		///  成员数量
		/// </summary>
		[AdvancedInspector.Descriptor(" 成员数量", " 成员数量")]
		public byte memberNum;
		/// <summary>
		///  成员上限
		/// </summary>
		[AdvancedInspector.Descriptor(" 成员上限", " 成员上限")]
		public byte maxMemberNum;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, teamId);
			BaseDLL.encode_uint32(buffer, ref pos_, target);
			masterInfo.encode(buffer, ref pos_);
			BaseDLL.encode_int8(buffer, ref pos_, memberNum);
			BaseDLL.encode_int8(buffer, ref pos_, maxMemberNum);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref teamId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref target);
			masterInfo.decode(buffer, ref pos_);
			BaseDLL.decode_int8(buffer, ref pos_, ref memberNum);
			BaseDLL.decode_int8(buffer, ref pos_, ref maxMemberNum);
		}


		#endregion

	}

}
