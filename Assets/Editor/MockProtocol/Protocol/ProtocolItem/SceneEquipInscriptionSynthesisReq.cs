using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 铭文合成
	/// </summary>
	[AdvancedInspector.Descriptor("铭文合成", "铭文合成")]
	public class SceneEquipInscriptionSynthesisReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501081;
		public UInt32 Sequence;
		/// <summary>
		///  材料id
		/// </summary>
		[AdvancedInspector.Descriptor(" 材料id", " 材料id")]
		public UInt32[] itemIDVec = new UInt32[0];

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)itemIDVec.Length);
			for(int i = 0; i < itemIDVec.Length; i++)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, itemIDVec[i]);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 itemIDVecCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref itemIDVecCnt);
			itemIDVec = new UInt32[itemIDVecCnt];
			for(int i = 0; i < itemIDVec.Length; i++)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemIDVec[i]);
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
