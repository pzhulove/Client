using System;
using System.Text;

namespace Mock.Protocol
{

	public class SceneSyncPetList : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 502201;
		public UInt32 Sequence;

		public UInt64 followPetId;

		public UInt64[] battlePets = new UInt64[0];

		public PetInfo[] petList = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, followPetId);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)battlePets.Length);
			for(int i = 0; i < battlePets.Length; i++)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, battlePets[i]);
			}
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)petList.Length);
			for(int i = 0; i < petList.Length; i++)
			{
				petList[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref followPetId);
			UInt16 battlePetsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref battlePetsCnt);
			battlePets = new UInt64[battlePetsCnt];
			for(int i = 0; i < battlePets.Length; i++)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref battlePets[i]);
			}
			UInt16 petListCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref petListCnt);
			petList = new PetInfo[petListCnt];
			for(int i = 0; i < petList.Length; i++)
			{
				petList[i] = new PetInfo();
				petList[i].decode(buffer, ref pos_);
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
