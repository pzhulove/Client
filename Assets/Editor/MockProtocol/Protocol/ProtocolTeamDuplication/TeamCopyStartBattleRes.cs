using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  开战返回
	/// </summary>
	[AdvancedInspector.Descriptor(" 开战返回", " 开战返回")]
	public class TeamCopyStartBattleRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 1100014;
		public UInt32 Sequence;

		public string roleName;

		public UInt32 retCode;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			byte[] roleNameBytes = StringHelper.StringToUTF8Bytes(roleName);
			BaseDLL.encode_string(buffer, ref pos_, roleNameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_uint32(buffer, ref pos_, retCode);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 roleNameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref roleNameLen);
			byte[] roleNameBytes = new byte[roleNameLen];
			for(int i = 0; i < roleNameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref roleNameBytes[i]);
			}
			roleName = StringHelper.BytesToString(roleNameBytes);
			BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
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
