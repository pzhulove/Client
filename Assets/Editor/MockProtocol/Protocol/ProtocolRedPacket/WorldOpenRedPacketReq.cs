using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  ����򿪺��(����Ѿ��򿪹��ˣ��Ǿ��ǲ鿴)
	/// </summary>
	[AdvancedInspector.Descriptor(" ����򿪺��(����Ѿ��򿪹��ˣ��Ǿ��ǲ鿴)", " ����򿪺��(����Ѿ��򿪹��ˣ��Ǿ��ǲ鿴)")]
	public class WorldOpenRedPacketReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 607308;
		public UInt32 Sequence;
		/// <summary>
		///  id
		/// </summary>
		[AdvancedInspector.Descriptor(" id", " id")]
		public UInt64 id;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, id);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref id);
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
