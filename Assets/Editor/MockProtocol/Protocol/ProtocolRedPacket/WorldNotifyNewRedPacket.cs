using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  通知有新红包可领
	/// </summary>
	[AdvancedInspector.Descriptor(" 通知有新红包可领", " 通知有新红包可领")]
	public class WorldNotifyNewRedPacket : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 607303;
		public UInt32 Sequence;
		/// <summary>
		///  红包基础信息
		/// </summary>
		[AdvancedInspector.Descriptor(" 红包基础信息", " 红包基础信息")]
		public RedPacketBaseEntry[] entry = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)entry.Length);
			for(int i = 0; i < entry.Length; i++)
			{
				entry[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 entryCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref entryCnt);
			entry = new RedPacketBaseEntry[entryCnt];
			for(int i = 0; i < entry.Length; i++)
			{
				entry[i] = new RedPacketBaseEntry();
				entry[i].decode(buffer, ref pos_);
			}
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
