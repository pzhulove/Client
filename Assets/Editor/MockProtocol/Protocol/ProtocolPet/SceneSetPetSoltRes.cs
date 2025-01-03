using System;
using System.Text;

namespace Mock.Protocol
{

	public class SceneSetPetSoltRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 502206;
		public UInt32 Sequence;

		public UInt32 result;

		public UInt64[] battlePets = new UInt64[0];

		public UInt64 followPetId;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, result);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)battlePets.Length);
			for(int i = 0; i < battlePets.Length; i++)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, battlePets[i]);
			}
			BaseDLL.encode_uint64(buffer, ref pos_, followPetId);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			UInt16 battlePetsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref battlePetsCnt);
			battlePets = new UInt64[battlePetsCnt];
			for(int i = 0; i < battlePets.Length; i++)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref battlePets[i]);
			}
			BaseDLL.decode_uint64(buffer, ref pos_, ref followPetId);
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
