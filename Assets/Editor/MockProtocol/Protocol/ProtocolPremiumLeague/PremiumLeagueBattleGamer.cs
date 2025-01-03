using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  淘汰赛玩家信息
	/// </summary>
	[AdvancedInspector.Descriptor(" 淘汰赛玩家信息", " 淘汰赛玩家信息")]
	public class PremiumLeagueBattleGamer : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  角色ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 角色ID", " 角色ID")]
		public UInt64 roleId;
		/// <summary>
		///  名字
		/// </summary>
		[AdvancedInspector.Descriptor(" 名字", " 名字")]
		public string name;
		/// <summary>
		///  职业
		/// </summary>
		[AdvancedInspector.Descriptor(" 职业", " 职业")]
		public byte occu;
		/// <summary>
		///  排名
		/// </summary>
		[AdvancedInspector.Descriptor(" 排名", " 排名")]
		public UInt32 ranking;
		/// <summary>
		///  胜场数
		/// </summary>
		[AdvancedInspector.Descriptor(" 胜场数", " 胜场数")]
		public UInt32 winNum;
		/// <summary>
		///  是否已经输了
		/// </summary>
		[AdvancedInspector.Descriptor(" 是否已经输了", " 是否已经输了")]
		public byte isLose;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, roleId);
			byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
			BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_int8(buffer, ref pos_, occu);
			BaseDLL.encode_uint32(buffer, ref pos_, ranking);
			BaseDLL.encode_uint32(buffer, ref pos_, winNum);
			BaseDLL.encode_int8(buffer, ref pos_, isLose);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
			UInt16 nameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
			byte[] nameBytes = new byte[nameLen];
			for(int i = 0; i < nameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
			}
			name = StringHelper.BytesToString(nameBytes);
			BaseDLL.decode_int8(buffer, ref pos_, ref occu);
			BaseDLL.decode_uint32(buffer, ref pos_, ref ranking);
			BaseDLL.decode_uint32(buffer, ref pos_, ref winNum);
			BaseDLL.decode_int8(buffer, ref pos_, ref isLose);
		}


		#endregion

	}

}
