using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  请求操作安全锁返回
	/// </summary>
	[AdvancedInspector.Descriptor(" 请求操作安全锁返回", " 请求操作安全锁返回")]
	public class WorldSecurityLockOpRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 608405;
		public UInt32 Sequence;
		/// <summary>
		///  返回值
		/// </summary>
		[AdvancedInspector.Descriptor(" 返回值", " 返回值")]
		public UInt32 ret;
		/// <summary>
		///  锁操作类型(LockOpType)
		/// </summary>
		[AdvancedInspector.Descriptor(" 锁操作类型(LockOpType)", " 锁操作类型(LockOpType)")]
		public UInt32 lockOpType;
		/// <summary>
		///  锁状态(SecurityLockState)
		/// </summary>
		[AdvancedInspector.Descriptor(" 锁状态(SecurityLockState)", " 锁状态(SecurityLockState)")]
		public UInt32 lockState;
		/// <summary>
		///  冻结时间
		/// </summary>
		[AdvancedInspector.Descriptor(" 冻结时间", " 冻结时间")]
		public UInt32 freezeTime;
		/// <summary>
		///  解冻时间
		/// </summary>
		[AdvancedInspector.Descriptor(" 解冻时间", " 解冻时间")]
		public UInt32 unFreezeTime;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, ret);
			BaseDLL.encode_uint32(buffer, ref pos_, lockOpType);
			BaseDLL.encode_uint32(buffer, ref pos_, lockState);
			BaseDLL.encode_uint32(buffer, ref pos_, freezeTime);
			BaseDLL.encode_uint32(buffer, ref pos_, unFreezeTime);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref ret);
			BaseDLL.decode_uint32(buffer, ref pos_, ref lockOpType);
			BaseDLL.decode_uint32(buffer, ref pos_, ref lockState);
			BaseDLL.decode_uint32(buffer, ref pos_, ref freezeTime);
			BaseDLL.decode_uint32(buffer, ref pos_, ref unFreezeTime);
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
