using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  返回队伍列表
	/// </summary>
	[AdvancedInspector.Descriptor(" 返回队伍列表", " 返回队伍列表")]
	public class WorldQueryTeamListRet : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601624;
		public UInt32 Sequence;
		/// <summary>
		///  队伍目标
		/// </summary>
		[AdvancedInspector.Descriptor(" 队伍目标", " 队伍目标")]
		public UInt32 targetId;

		public TeamBaseInfo[] teamList = null;

		public UInt16 pos;

		public UInt16 maxNum;
		/// <summary>
		///  参数
		/// </summary>
		[AdvancedInspector.Descriptor(" 参数", " 参数")]
		public byte param;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, targetId);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)teamList.Length);
			for(int i = 0; i < teamList.Length; i++)
			{
				teamList[i].encode(buffer, ref pos_);
			}
			BaseDLL.encode_uint16(buffer, ref pos_, pos);
			BaseDLL.encode_uint16(buffer, ref pos_, maxNum);
			BaseDLL.encode_int8(buffer, ref pos_, param);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref targetId);
			UInt16 teamListCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref teamListCnt);
			teamList = new TeamBaseInfo[teamListCnt];
			for(int i = 0; i < teamList.Length; i++)
			{
				teamList[i] = new TeamBaseInfo();
				teamList[i].decode(buffer, ref pos_);
			}
			BaseDLL.decode_uint16(buffer, ref pos_, ref pos);
			BaseDLL.decode_uint16(buffer, ref pos_, ref maxNum);
			BaseDLL.decode_int8(buffer, ref pos_, ref param);
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
