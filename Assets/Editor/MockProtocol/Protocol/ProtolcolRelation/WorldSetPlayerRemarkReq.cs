using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// client->world  设置玩家备注请求
	/// </summary>
	[AdvancedInspector.Descriptor("client->world  设置玩家备注请求", "client->world  设置玩家备注请求")]
	public class WorldSetPlayerRemarkReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601737;
		public UInt32 Sequence;

		public UInt64 roleId;

		public string remark;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, roleId);
			byte[] remarkBytes = StringHelper.StringToUTF8Bytes(remark);
			BaseDLL.encode_string(buffer, ref pos_, remarkBytes, (UInt16)(buffer.Length - pos_));
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
			UInt16 remarkLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref remarkLen);
			byte[] remarkBytes = new byte[remarkLen];
			for(int i = 0; i < remarkLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref remarkBytes[i]);
			}
			remark = StringHelper.BytesToString(remarkBytes);
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
