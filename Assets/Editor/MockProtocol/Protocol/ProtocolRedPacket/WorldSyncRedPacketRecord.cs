using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// world->client 同步红包记录
	/// </summary>
	[AdvancedInspector.Descriptor("world->client 同步红包记录", "world->client 同步红包记录")]
	public class WorldSyncRedPacketRecord : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 607312;
		public UInt32 Sequence;

		public RedPacketRecord[] adds = null;

		public UInt64[] dels = new UInt64[0];

		public RedPacketRecord[] updates = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)adds.Length);
			for(int i = 0; i < adds.Length; i++)
			{
				adds[i].encode(buffer, ref pos_);
			}
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)dels.Length);
			for(int i = 0; i < dels.Length; i++)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, dels[i]);
			}
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)updates.Length);
			for(int i = 0; i < updates.Length; i++)
			{
				updates[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 addsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref addsCnt);
			adds = new RedPacketRecord[addsCnt];
			for(int i = 0; i < adds.Length; i++)
			{
				adds[i] = new RedPacketRecord();
				adds[i].decode(buffer, ref pos_);
			}
			UInt16 delsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref delsCnt);
			dels = new UInt64[delsCnt];
			for(int i = 0; i < dels.Length; i++)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref dels[i]);
			}
			UInt16 updatesCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref updatesCnt);
			updates = new RedPacketRecord[updatesCnt];
			for(int i = 0; i < updates.Length; i++)
			{
				updates[i] = new RedPacketRecord();
				updates[i].decode(buffer, ref pos_);
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
