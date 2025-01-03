using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  请求修改安全锁密码返回
	/// </summary>
	[AdvancedInspector.Descriptor(" 请求修改安全锁密码返回", " 请求修改安全锁密码返回")]
	public class WorldChangeSecurityPasswdRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 608407;
		public UInt32 Sequence;
		/// <summary>
		///  返回值
		/// </summary>
		[AdvancedInspector.Descriptor(" 返回值", " 返回值")]
		public UInt32 ret;
		/// <summary>
		///  错误次数
		/// </summary>
		[AdvancedInspector.Descriptor(" 错误次数", " 错误次数")]
		public UInt32 errNum;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, ret);
			BaseDLL.encode_uint32(buffer, ref pos_, errNum);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref ret);
			BaseDLL.decode_uint32(buffer, ref pos_, ref errNum);
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
