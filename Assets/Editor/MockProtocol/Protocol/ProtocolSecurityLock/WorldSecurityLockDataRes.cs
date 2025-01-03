using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  请求安全锁信息返回
	/// </summary>
	[AdvancedInspector.Descriptor(" 请求安全锁信息返回", " 请求安全锁信息返回")]
	public class WorldSecurityLockDataRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 608403;
		public UInt32 Sequence;
		/// <summary>
		///  锁状态(SecurityLockState)
		/// </summary>
		[AdvancedInspector.Descriptor(" 锁状态(SecurityLockState)", " 锁状态(SecurityLockState)")]
		public UInt32 lockState;
		/// <summary>
		///  是否常用设备
		/// </summary>
		[AdvancedInspector.Descriptor(" 是否常用设备", " 是否常用设备")]
		public UInt32 isCommonDev;
		/// <summary>
		///  是否上过锁
		/// </summary>
		[AdvancedInspector.Descriptor(" 是否上过锁", " 是否上过锁")]
		public UInt32 isUseLock;
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
			BaseDLL.encode_uint32(buffer, ref pos_, lockState);
			BaseDLL.encode_uint32(buffer, ref pos_, isCommonDev);
			BaseDLL.encode_uint32(buffer, ref pos_, isUseLock);
			BaseDLL.encode_uint32(buffer, ref pos_, freezeTime);
			BaseDLL.encode_uint32(buffer, ref pos_, unFreezeTime);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref lockState);
			BaseDLL.decode_uint32(buffer, ref pos_, ref isCommonDev);
			BaseDLL.decode_uint32(buffer, ref pos_, ref isUseLock);
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
