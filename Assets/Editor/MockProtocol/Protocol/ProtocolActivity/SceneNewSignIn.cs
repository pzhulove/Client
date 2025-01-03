using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 新自然月签到
	/// </summary>
	[AdvancedInspector.Descriptor("新自然月签到", "新自然月签到")]
	public class SceneNewSignIn : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501161;
		public UInt32 Sequence;

		public byte day;

		public byte isAll;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, day);
			BaseDLL.encode_int8(buffer, ref pos_, isAll);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref day);
			BaseDLL.decode_int8(buffer, ref pos_, ref isAll);
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
