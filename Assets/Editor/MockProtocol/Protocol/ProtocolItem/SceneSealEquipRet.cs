using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  装备uid
	/// </summary>
	[AdvancedInspector.Descriptor(" 装备uid", " 装备uid")]
	public class SceneSealEquipRet : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 500938;
		public UInt32 Sequence;

		public UInt32 code;
		/// <summary>
		/// 返回码
		/// </summary>
		/// <summary>
		/// 铭文ID数组
		/// </summary>
		[AdvancedInspector.Descriptor("铭文ID数组", "铭文ID数组")]
		public UInt32[] inscriptionIds = new UInt32[0];

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, code);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)inscriptionIds.Length);
			for(int i = 0; i < inscriptionIds.Length; i++)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, inscriptionIds[i]);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			UInt16 inscriptionIdsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref inscriptionIdsCnt);
			inscriptionIds = new UInt32[inscriptionIdsCnt];
			for(int i = 0; i < inscriptionIds.Length; i++)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref inscriptionIds[i]);
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
