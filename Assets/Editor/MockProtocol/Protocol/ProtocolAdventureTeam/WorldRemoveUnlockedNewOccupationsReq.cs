using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  client->world 请求清除解锁的新职业
	/// </summary>
	[AdvancedInspector.Descriptor(" client->world 请求清除解锁的新职业", " client->world 请求清除解锁的新职业")]
	public class WorldRemoveUnlockedNewOccupationsReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 608626;
		public UInt32 Sequence;
		/// <summary>
		///  新职业
		/// </summary>
		[AdvancedInspector.Descriptor(" 新职业", " 新职业")]
		public byte[] newOccus = new byte[0];

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)newOccus.Length);
			for(int i = 0; i < newOccus.Length; i++)
			{
				BaseDLL.encode_int8(buffer, ref pos_, newOccus[i]);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 newOccusCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref newOccusCnt);
			newOccus = new byte[newOccusCnt];
			for(int i = 0; i < newOccus.Length; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref newOccus[i]);
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
