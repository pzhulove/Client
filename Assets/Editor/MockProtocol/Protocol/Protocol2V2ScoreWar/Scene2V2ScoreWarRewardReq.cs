using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// client->scene 请求积分赛奖励
	/// </summary>
	[AdvancedInspector.Descriptor("client->scene 请求积分赛奖励", "client->scene 请求积分赛奖励")]
	public class Scene2V2ScoreWarRewardReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 509603;
		public UInt32 Sequence;
		/// <summary>
		/// 奖励ID
		/// </summary>
		[AdvancedInspector.Descriptor("奖励ID", "奖励ID")]
		public byte rewardId;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, rewardId);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref rewardId);
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
