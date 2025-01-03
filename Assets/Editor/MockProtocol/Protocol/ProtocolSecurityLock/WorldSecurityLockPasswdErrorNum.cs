using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  安全锁密码错误次数
	/// </summary>
	[AdvancedInspector.Descriptor(" 安全锁密码错误次数", " 安全锁密码错误次数")]
	public class WorldSecurityLockPasswdErrorNum : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 608411;
		public UInt32 Sequence;

		public UInt32 error_num;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, error_num);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref error_num);
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
