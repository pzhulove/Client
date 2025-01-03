using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  通知删除红包
	/// </summary>
	[AdvancedInspector.Descriptor(" 通知删除红包", " 通知删除红包")]
	public class WorldNotifyDelRedPacket : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 607304;
		public UInt32 Sequence;
		/// <summary>
		///  需要删除的红包ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 需要删除的红包ID", " 需要删除的红包ID")]
		public UInt64[] redPacketList = new UInt64[0];

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)redPacketList.Length);
			for(int i = 0; i < redPacketList.Length; i++)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, redPacketList[i]);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 redPacketListCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref redPacketListCnt);
			redPacketList = new UInt64[redPacketListCnt];
			for(int i = 0; i < redPacketList.Length; i++)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref redPacketList[i]);
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
