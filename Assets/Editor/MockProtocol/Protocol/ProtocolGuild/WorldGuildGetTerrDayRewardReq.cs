using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  公会领地每日奖励请求
	/// </summary>
	[AdvancedInspector.Descriptor(" 公会领地每日奖励请求", " 公会领地每日奖励请求")]
	public class WorldGuildGetTerrDayRewardReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601994;
		public UInt32 Sequence;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
		}

		public void decode(byte[] buffer, ref int pos_)
		{
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
