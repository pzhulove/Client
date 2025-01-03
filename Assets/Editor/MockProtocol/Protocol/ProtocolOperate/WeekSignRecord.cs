using System;
using System.Text;

namespace Mock.Protocol
{

	public class WeekSignRecord : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  服务器名字
		/// </summary>
		[AdvancedInspector.Descriptor(" 服务器名字", " 服务器名字")]
		public string serverName;
		/// <summary>
		///  玩家名字
		/// </summary>
		[AdvancedInspector.Descriptor(" 玩家名字", " 玩家名字")]
		public string roleName;
		/// <summary>
		///  道具ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 道具ID", " 道具ID")]
		public UInt32 itemId;
		/// <summary>
		///  道具数量
		/// </summary>
		[AdvancedInspector.Descriptor(" 道具数量", " 道具数量")]
		public UInt32 itemNum;
		/// <summary>
		///  创建时间
		/// </summary>
		[AdvancedInspector.Descriptor(" 创建时间", " 创建时间")]
		public UInt32 createTime;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			byte[] serverNameBytes = StringHelper.StringToUTF8Bytes(serverName);
			BaseDLL.encode_string(buffer, ref pos_, serverNameBytes, (UInt16)(buffer.Length - pos_));
			byte[] roleNameBytes = StringHelper.StringToUTF8Bytes(roleName);
			BaseDLL.encode_string(buffer, ref pos_, roleNameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_uint32(buffer, ref pos_, itemId);
			BaseDLL.encode_uint32(buffer, ref pos_, itemNum);
			BaseDLL.encode_uint32(buffer, ref pos_, createTime);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 serverNameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref serverNameLen);
			byte[] serverNameBytes = new byte[serverNameLen];
			for(int i = 0; i < serverNameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref serverNameBytes[i]);
			}
			serverName = StringHelper.BytesToString(serverNameBytes);
			UInt16 roleNameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref roleNameLen);
			byte[] roleNameBytes = new byte[roleNameLen];
			for(int i = 0; i < roleNameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref roleNameBytes[i]);
			}
			roleName = StringHelper.BytesToString(roleNameBytes);
			BaseDLL.decode_uint32(buffer, ref pos_, ref itemId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref itemNum);
			BaseDLL.decode_uint32(buffer, ref pos_, ref createTime);
		}


		#endregion

	}

}
