using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  world->client  通知功能解锁
	/// </summary>
	[AdvancedInspector.Descriptor(" world->client  通知功能解锁", " world->client  通知功能解锁")]
	public class WorldSyncFuncUnlock : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601201;
		public UInt32 Sequence;

		public byte funcId;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, funcId);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref funcId);
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
