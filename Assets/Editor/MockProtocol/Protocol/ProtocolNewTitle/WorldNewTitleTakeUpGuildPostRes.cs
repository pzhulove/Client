using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// world->client	穿上公会职位的头衔返回
	/// </summary>
	[AdvancedInspector.Descriptor("world->client	穿上公会职位的头衔返回", "world->client	穿上公会职位的头衔返回")]
	public class WorldNewTitleTakeUpGuildPostRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 609212;
		public UInt32 Sequence;

		public UInt32 res;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, res);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref res);
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
