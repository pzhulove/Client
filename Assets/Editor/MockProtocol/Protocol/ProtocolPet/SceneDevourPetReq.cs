using System;
using System.Text;

namespace Mock.Protocol
{

	public class SceneDevourPetReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 502215;
		public UInt32 Sequence;

		public UInt64 id;

		public UInt64[] petIds = new UInt64[0];

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, id);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)petIds.Length);
			for(int i = 0; i < petIds.Length; i++)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, petIds[i]);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref id);
			UInt16 petIdsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref petIdsCnt);
			petIds = new UInt64[petIdsCnt];
			for(int i = 0; i < petIds.Length; i++)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref petIds[i]);
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
