using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  world->client 查询拥有的职业返回
	/// </summary>
	[AdvancedInspector.Descriptor(" world->client 查询拥有的职业返回", " world->client 查询拥有的职业返回")]
	public class WorldQueryOwnOccupationsRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 608624;
		public UInt32 Sequence;
		/// <summary>
		///  错误码
		/// </summary>
		[AdvancedInspector.Descriptor(" 错误码", " 错误码")]
		public UInt32 resCode;
		/// <summary>
		///  职业集
		/// </summary>
		[AdvancedInspector.Descriptor(" 职业集", " 职业集")]
		public byte[] occus = new byte[0];

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, resCode);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)occus.Length);
			for(int i = 0; i < occus.Length; i++)
			{
				BaseDLL.encode_int8(buffer, ref pos_, occus[i]);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref resCode);
			UInt16 occusCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref occusCnt);
			occus = new byte[occusCnt];
			for(int i = 0; i < occus.Length; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref occus[i]);
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
