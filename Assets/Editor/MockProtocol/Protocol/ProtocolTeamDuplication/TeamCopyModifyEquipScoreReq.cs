using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  修改队伍装备评分请求
	/// </summary>
	[AdvancedInspector.Descriptor(" 修改队伍装备评分请求", " 修改队伍装备评分请求")]
	public class TeamCopyModifyEquipScoreReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 1100076;
		public UInt32 Sequence;

		public UInt32 equipScore;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, equipScore);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref equipScore);
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
