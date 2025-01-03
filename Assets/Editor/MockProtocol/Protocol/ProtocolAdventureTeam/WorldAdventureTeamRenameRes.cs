using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  world->client 冒险队队名修改返回
	/// </summary>
	[AdvancedInspector.Descriptor(" world->client 冒险队队名修改返回", " world->client 冒险队队名修改返回")]
	public class WorldAdventureTeamRenameRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 608602;
		public UInt32 Sequence;
		/// <summary>
		///  错误码
		/// </summary>
		[AdvancedInspector.Descriptor(" 错误码", " 错误码")]
		public UInt32 resCode;
		/// <summary>
		///  要修改的新名字
		/// </summary>
		[AdvancedInspector.Descriptor(" 要修改的新名字", " 要修改的新名字")]
		public string newName;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, resCode);
			byte[] newNameBytes = StringHelper.StringToUTF8Bytes(newName);
			BaseDLL.encode_string(buffer, ref pos_, newNameBytes, (UInt16)(buffer.Length - pos_));
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref resCode);
			UInt16 newNameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref newNameLen);
			byte[] newNameBytes = new byte[newNameLen];
			for(int i = 0; i < newNameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref newNameBytes[i]);
			}
			newName = StringHelper.BytesToString(newNameBytes);
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
