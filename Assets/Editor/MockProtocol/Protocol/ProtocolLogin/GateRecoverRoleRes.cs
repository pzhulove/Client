using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  恢复角色返回
	/// </summary>
	[AdvancedInspector.Descriptor(" 恢复角色返回", " 恢复角色返回")]
	public class GateRecoverRoleRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 300314;
		public UInt32 Sequence;

		public UInt64 roleId;

		public UInt32 result;

		public string roleUpdateLimit;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, roleId);
			BaseDLL.encode_uint32(buffer, ref pos_, result);
			byte[] roleUpdateLimitBytes = StringHelper.StringToUTF8Bytes(roleUpdateLimit);
			BaseDLL.encode_string(buffer, ref pos_, roleUpdateLimitBytes, (UInt16)(buffer.Length - pos_));
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			UInt16 roleUpdateLimitLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref roleUpdateLimitLen);
			byte[] roleUpdateLimitBytes = new byte[roleUpdateLimitLen];
			for(int i = 0; i < roleUpdateLimitLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref roleUpdateLimitBytes[i]);
			}
			roleUpdateLimit = StringHelper.BytesToString(roleUpdateLimitBytes);
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
