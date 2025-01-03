using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  ֪ͨ����º��
	/// </summary>
	[AdvancedInspector.Descriptor(" ֪ͨ����º��", " ֪ͨ����º��")]
	public class WorldNotifyGotNewRedPacket : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 607302;
		public UInt32 Sequence;
		/// <summary>
		///  ���������Ϣ
		/// </summary>
		[AdvancedInspector.Descriptor(" ���������Ϣ", " ���������Ϣ")]
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
