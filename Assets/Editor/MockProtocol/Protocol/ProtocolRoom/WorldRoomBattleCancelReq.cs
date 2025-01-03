using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  client->server 请求取消匹配战斗
	/// </summary>
	[AdvancedInspector.Descriptor(" client->server 请求取消匹配战斗", " client->server 请求取消匹配战斗")]
	public class WorldRoomBattleCancelReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 607837;
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
