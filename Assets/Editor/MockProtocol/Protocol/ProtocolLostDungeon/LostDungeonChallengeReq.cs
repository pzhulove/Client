using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// PVP战斗中
	/// </summary>
	/// <summary>
	///  挑战迷失地牢请求
	/// </summary>
	[AdvancedInspector.Descriptor(" 挑战迷失地牢请求", " 挑战迷失地牢请求")]
	public class LostDungeonChallengeReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 510001;
		public UInt32 Sequence;
		/// <summary>
		///  层数
		/// </summary>
		[AdvancedInspector.Descriptor(" 层数", " 层数")]
		public UInt32 floor;
		/// <summary>
		///  难度
		/// </summary>
		[AdvancedInspector.Descriptor(" 难度", " 难度")]
		public byte hardType;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, floor);
			BaseDLL.encode_int8(buffer, ref pos_, hardType);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref floor);
			BaseDLL.decode_int8(buffer, ref pos_, ref hardType);
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
