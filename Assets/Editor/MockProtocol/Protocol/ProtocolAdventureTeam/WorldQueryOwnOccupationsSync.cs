using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  world->client 同步拥有的新职业
	/// </summary>
	[AdvancedInspector.Descriptor(" world->client 同步拥有的新职业", " world->client 同步拥有的新职业")]
	public class WorldQueryOwnOccupationsSync : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 608625;
		public UInt32 Sequence;
		/// <summary>
		///  拥有的新职业
		/// </summary>
		[AdvancedInspector.Descriptor(" 拥有的新职业", " 拥有的新职业")]
		public byte[] ownNewOccus = new byte[0];

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)ownNewOccus.Length);
			for(int i = 0; i < ownNewOccus.Length; i++)
			{
				BaseDLL.encode_int8(buffer, ref pos_, ownNewOccus[i]);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 ownNewOccusCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref ownNewOccusCnt);
			ownNewOccus = new byte[ownNewOccusCnt];
			for(int i = 0; i < ownNewOccus.Length; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref ownNewOccus[i]);
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
