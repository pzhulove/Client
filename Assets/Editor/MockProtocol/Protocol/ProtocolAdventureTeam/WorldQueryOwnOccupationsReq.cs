using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  client->world 请求查询拥有的职业
	/// </summary>
	[AdvancedInspector.Descriptor(" client->world 请求查询拥有的职业", " client->world 请求查询拥有的职业")]
	public class WorldQueryOwnOccupationsReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 608623;
		public UInt32 Sequence;
		/// <summary>
		///  基础职业
		/// </summary>
		[AdvancedInspector.Descriptor(" 基础职业", " 基础职业")]
		public byte[] baseOccus = new byte[0];

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)baseOccus.Length);
			for(int i = 0; i < baseOccus.Length; i++)
			{
				BaseDLL.encode_int8(buffer, ref pos_, baseOccus[i]);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 baseOccusCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref baseOccusCnt);
			baseOccus = new byte[baseOccusCnt];
			for(int i = 0; i < baseOccus.Length; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref baseOccus[i]);
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
