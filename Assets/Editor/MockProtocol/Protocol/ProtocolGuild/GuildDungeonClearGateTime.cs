using System;
using System.Text;

namespace Mock.Protocol
{

	public class GuildDungeonClearGateTime : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  公会名字
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会名字", " 公会名字")]
		public string guildName;
		/// <summary>
		///  通关用时
		/// </summary>
		[AdvancedInspector.Descriptor(" 通关用时", " 通关用时")]
		public UInt64 spendTime;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			byte[] guildNameBytes = StringHelper.StringToUTF8Bytes(guildName);
			BaseDLL.encode_string(buffer, ref pos_, guildNameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_uint64(buffer, ref pos_, spendTime);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 guildNameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref guildNameLen);
			byte[] guildNameBytes = new byte[guildNameLen];
			for(int i = 0; i < guildNameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref guildNameBytes[i]);
			}
			guildName = StringHelper.BytesToString(guildNameBytes);
			BaseDLL.decode_uint64(buffer, ref pos_, ref spendTime);
		}


		#endregion

	}

}
