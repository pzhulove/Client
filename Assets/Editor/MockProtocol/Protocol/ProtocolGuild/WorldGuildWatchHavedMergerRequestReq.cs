using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// client ->  world 查看本公会收到的兼并申请列表
	/// </summary>
	[AdvancedInspector.Descriptor("client ->  world 查看本公会收到的兼并申请列表", "client ->  world 查看本公会收到的兼并申请列表")]
	public class WorldGuildWatchHavedMergerRequestReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601983;
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
