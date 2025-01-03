using System;
using System.Text;

namespace Mock.Protocol
{

	public class SceneDungeonOpenChestRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 506814;
		public UInt32 Sequence;

		public UInt64 owner;

		public byte pos;

		public DungeonChest chest = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, owner);
			BaseDLL.encode_int8(buffer, ref pos_, pos);
			chest.encode(buffer, ref pos_);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref owner);
			BaseDLL.decode_int8(buffer, ref pos_, ref pos);
			chest.decode(buffer, ref pos_);
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
