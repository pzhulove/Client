using System;
using System.Text;

namespace Mock.Protocol
{

	public class SceneDungeonRewardReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 506809;
		public UInt32 Sequence;

		public UInt32 lastFrame;

		public UInt32[] dropItemIds = new UInt32[0];

		public byte[] md5 = new byte[16];

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, lastFrame);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)dropItemIds.Length);
			for(int i = 0; i < dropItemIds.Length; i++)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, dropItemIds[i]);
			}
			for(int i = 0; i < md5.Length; i++)
			{
				BaseDLL.encode_int8(buffer, ref pos_, md5[i]);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref lastFrame);
			UInt16 dropItemIdsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref dropItemIdsCnt);
			dropItemIds = new UInt32[dropItemIdsCnt];
			for(int i = 0; i < dropItemIds.Length; i++)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref dropItemIds[i]);
			}
			for(int i = 0; i < md5.Length; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref md5[i]);
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
