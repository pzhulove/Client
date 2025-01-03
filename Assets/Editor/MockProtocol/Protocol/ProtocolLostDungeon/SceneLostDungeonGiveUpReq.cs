using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// client->scene 放弃爬塔请求
	/// </summary>
	[AdvancedInspector.Descriptor("client->scene 放弃爬塔请求", "client->scene 放弃爬塔请求")]
	public class SceneLostDungeonGiveUpReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 510027;
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
