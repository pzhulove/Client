using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  同步玩家登陆状态
	/// </summary>
	[AdvancedInspector.Descriptor(" 同步玩家登陆状态", " 同步玩家登陆状态")]
	public class SyncPlayerLoginStatus : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 600308;
		public UInt32 Sequence;
		/// <summary>
		///  对应枚举PlayerLoginStatus
		/// </summary>
		[AdvancedInspector.Descriptor(" 对应枚举PlayerLoginStatus", " 对应枚举PlayerLoginStatus")]
		public byte playerLoginStatus;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, playerLoginStatus);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref playerLoginStatus);
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
