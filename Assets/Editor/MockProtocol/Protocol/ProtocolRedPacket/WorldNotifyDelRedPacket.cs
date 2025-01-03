using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  ֪ͨɾ�����
	/// </summary>
	[AdvancedInspector.Descriptor(" ֪ͨɾ�����", " ֪ͨɾ�����")]
	public class WorldNotifyDelRedPacket : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 607304;
		public UInt32 Sequence;
		/// <summary>
		///  ��Ҫɾ���ĺ��ID
		/// </summary>
		[AdvancedInspector.Descriptor(" ��Ҫɾ���ĺ��ID", " ��Ҫɾ���ĺ��ID")]
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
