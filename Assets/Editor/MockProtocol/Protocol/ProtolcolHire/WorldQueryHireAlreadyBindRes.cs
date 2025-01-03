using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 查询有没有在别的服绑定返回
	/// </summary>
	[AdvancedInspector.Descriptor("查询有没有在别的服绑定返回", "查询有没有在别的服绑定返回")]
	public class WorldQueryHireAlreadyBindRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601798;
		public UInt32 Sequence;

		public UInt32 errorCode;

		public UInt32 accid;

		public UInt32 zone;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, errorCode);
			BaseDLL.encode_uint32(buffer, ref pos_, accid);
			BaseDLL.encode_uint32(buffer, ref pos_, zone);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref errorCode);
			BaseDLL.decode_uint32(buffer, ref pos_, ref accid);
			BaseDLL.decode_uint32(buffer, ref pos_, ref zone);
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
