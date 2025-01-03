using System;
using System.Text;

namespace Mock.Protocol
{

	public class SceneSetTaskItemReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501133;
		public UInt32 Sequence;

		public UInt32 taskId;

		public UInt64[] itemIds = new UInt64[0];

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, taskId);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)itemIds.Length);
			for(int i = 0; i < itemIds.Length; i++)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, itemIds[i]);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref taskId);
			UInt16 itemIdsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref itemIdsCnt);
			itemIds = new UInt64[itemIdsCnt];
			for(int i = 0; i < itemIds.Length; i++)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref itemIds[i]);
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
