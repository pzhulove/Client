using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  server->client 同步积分赛信息
	/// </summary>
	[AdvancedInspector.Descriptor(" server->client 同步积分赛信息", " server->client 同步积分赛信息")]
	public class Scene2V2SyncScoreWarInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 509601;
		public UInt32 Sequence;

		public byte status;

		public UInt32 statusEndTime;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, status);
			BaseDLL.encode_uint32(buffer, ref pos_, statusEndTime);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref status);
			BaseDLL.decode_uint32(buffer, ref pos_, ref statusEndTime);
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
