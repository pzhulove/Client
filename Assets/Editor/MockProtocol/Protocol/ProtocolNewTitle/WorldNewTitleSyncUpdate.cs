using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 唯一id
	/// </summary>
	/// <summary>
	/// world->client 同步新增或删除头衔
	/// </summary>
	[AdvancedInspector.Descriptor("world->client 同步新增或删除头衔", "world->client 同步新增或删除头衔")]
	public class WorldNewTitleSyncUpdate : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 609206;
		public UInt32 Sequence;

		public PlayerTitleInfo[] adds = null;
		/// <summary>
		/// 新增
		/// </summary>
		[AdvancedInspector.Descriptor("新增", "新增")]
		public UInt64[] dels = new UInt64[0];

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
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 addsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref addsCnt);
			adds = new PlayerTitleInfo[addsCnt];
			for(int i = 0; i < adds.Length; i++)
			{
				adds[i] = new PlayerTitleInfo();
				adds[i].decode(buffer, ref pos_);
			}
			UInt16 delsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref delsCnt);
			dels = new UInt64[delsCnt];
			for(int i = 0; i < dels.Length; i++)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref dels[i]);
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
