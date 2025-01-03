using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  通知获得新红包
	/// </summary>
	[AdvancedInspector.Descriptor(" 通知获得新红包", " 通知获得新红包")]
	public class WorldNotifyGotNewRedPacket : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 607302;
		public UInt32 Sequence;
		/// <summary>
		///  红包基础信息
		/// </summary>
		[AdvancedInspector.Descriptor(" 红包基础信息", " 红包基础信息")]
		public RedPacketBaseEntry entry = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			entry.encode(buffer, ref pos_);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			entry.decode(buffer, ref pos_);
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
