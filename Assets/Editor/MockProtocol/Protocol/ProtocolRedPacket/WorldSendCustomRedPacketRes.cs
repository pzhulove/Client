using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  ���ԷѺ������
	/// </summary>
	[AdvancedInspector.Descriptor(" ���ԷѺ������", " ���ԷѺ������")]
	public class WorldSendCustomRedPacketRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 607311;
		public UInt32 Sequence;
		/// <summary>
		///  result
		/// </summary>
		[AdvancedInspector.Descriptor(" result", " result")]
		public UInt32 result;
		/// <summary>
		///  ���ID
		/// </summary>
		[AdvancedInspector.Descriptor(" ���ID", " ���ID")]
		public UInt64 redPacketId;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, result);
			BaseDLL.encode_uint64(buffer, ref pos_, redPacketId);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			BaseDLL.decode_uint64(buffer, ref pos_, ref redPacketId);
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
