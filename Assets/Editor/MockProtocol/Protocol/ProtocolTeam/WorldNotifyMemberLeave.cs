using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  通知成员离开
	/// </summary>
	[AdvancedInspector.Descriptor(" 通知成员离开", " 通知成员离开")]
	public class WorldNotifyMemberLeave : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601604;
		public UInt32 Sequence;
		/// <summary>
		///  队员ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 队员ID", " 队员ID")]
		public UInt64 id;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, id);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref id);
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
