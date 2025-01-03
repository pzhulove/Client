using System;
using System.Text;

namespace Mock.Protocol
{

	public class BattleServerSpecifyPkRobot : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 2200011;
		public UInt32 Sequence;

		public byte hard;

		public byte occu;

		public byte ai;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, hard);
			BaseDLL.encode_int8(buffer, ref pos_, occu);
			BaseDLL.encode_int8(buffer, ref pos_, ai);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref hard);
			BaseDLL.decode_int8(buffer, ref pos_, ref occu);
			BaseDLL.decode_int8(buffer, ref pos_, ref ai);
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
