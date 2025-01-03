using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  创建队伍
	/// </summary>
	[AdvancedInspector.Descriptor(" 创建队伍", " 创建队伍")]
	public class WorldCreateTeam : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601610;
		public UInt32 Sequence;
		/// <summary>
		///  队伍目标
		/// </summary>
		[AdvancedInspector.Descriptor(" 队伍目标", " 队伍目标")]
		public UInt32 target;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, target);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref target);
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
