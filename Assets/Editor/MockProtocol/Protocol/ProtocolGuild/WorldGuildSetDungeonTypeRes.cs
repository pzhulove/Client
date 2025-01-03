using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  设置公会副本难度返回
	/// </summary>
	[AdvancedInspector.Descriptor(" 设置公会副本难度返回", " 设置公会副本难度返回")]
	public class WorldGuildSetDungeonTypeRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601993;
		public UInt32 Sequence;

		public UInt32 result;

		public UInt32 dungeonType;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, result);
			BaseDLL.encode_uint32(buffer, ref pos_, dungeonType);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			BaseDLL.decode_uint32(buffer, ref pos_, ref dungeonType);
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
