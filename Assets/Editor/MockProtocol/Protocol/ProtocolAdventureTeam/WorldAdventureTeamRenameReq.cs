using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  client->world 冒险队队名修改请求
	/// </summary>
	[AdvancedInspector.Descriptor(" client->world 冒险队队名修改请求", " client->world 冒险队队名修改请求")]
	public class WorldAdventureTeamRenameReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 608601;
		public UInt32 Sequence;
		/// <summary>
		///  要修改的新名字
		/// </summary>
		[AdvancedInspector.Descriptor(" 要修改的新名字", " 要修改的新名字")]
		public string newName;
		/// <summary>
		///  需要消耗的道具
		/// </summary>
		[AdvancedInspector.Descriptor(" 需要消耗的道具", " 需要消耗的道具")]
		public UInt64 costItemUId;

		public UInt32 costItemDataId;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			byte[] newNameBytes = StringHelper.StringToUTF8Bytes(newName);
			BaseDLL.encode_string(buffer, ref pos_, newNameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_uint64(buffer, ref pos_, costItemUId);
			BaseDLL.encode_uint32(buffer, ref pos_, costItemDataId);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 newNameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref newNameLen);
			byte[] newNameBytes = new byte[newNameLen];
			for(int i = 0; i < newNameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref newNameBytes[i]);
			}
			newName = StringHelper.BytesToString(newNameBytes);
			BaseDLL.decode_uint64(buffer, ref pos_, ref costItemUId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref costItemDataId);
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
