using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// client->world 领取通行证等级奖励
	/// </summary>
	[AdvancedInspector.Descriptor("client->world 领取通行证等级奖励", "client->world 领取通行证等级奖励")]
	public class WorldAventurePassRewardReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 609509;
		public UInt32 Sequence;

		public UInt32 lv;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, lv);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref lv);
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
