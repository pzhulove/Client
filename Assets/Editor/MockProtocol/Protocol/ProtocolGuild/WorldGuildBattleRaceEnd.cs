using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  单次战斗结束
	/// </summary>
	[AdvancedInspector.Descriptor(" 单次战斗结束", " 单次战斗结束")]
	public class WorldGuildBattleRaceEnd : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601953;
		public UInt32 Sequence;

		public byte result;

		public UInt32 oldScore;

		public UInt32 newScore;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, result);
			BaseDLL.encode_uint32(buffer, ref pos_, oldScore);
			BaseDLL.encode_uint32(buffer, ref pos_, newScore);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref result);
			BaseDLL.decode_uint32(buffer, ref pos_, ref oldScore);
			BaseDLL.decode_uint32(buffer, ref pos_, ref newScore);
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
