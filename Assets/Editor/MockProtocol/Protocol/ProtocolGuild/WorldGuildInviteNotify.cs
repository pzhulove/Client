using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  公会邀请通知，有别的玩家邀请加入公会
	/// </summary>
	[AdvancedInspector.Descriptor(" 公会邀请通知，有别的玩家邀请加入公会", " 公会邀请通知，有别的玩家邀请加入公会")]
	public class WorldGuildInviteNotify : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601957;
		public UInt32 Sequence;
		/// <summary>
		///  邀请者ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 邀请者ID", " 邀请者ID")]
		public UInt64 inviterId;
		/// <summary>
		///  邀请者名字
		/// </summary>
		[AdvancedInspector.Descriptor(" 邀请者名字", " 邀请者名字")]
		public string inviterName;
		/// <summary>
		///  邀请者职业
		/// </summary>
		[AdvancedInspector.Descriptor(" 邀请者职业", " 邀请者职业")]
		public byte inviterOccu;
		/// <summary>
		///  邀请者等级
		/// </summary>
		[AdvancedInspector.Descriptor(" 邀请者等级", " 邀请者等级")]
		public UInt16 inviterLevel;
		/// <summary>
		///  邀请者VIP等级
		/// </summary>
		[AdvancedInspector.Descriptor(" 邀请者VIP等级", " 邀请者VIP等级")]
		public byte inviterVipLevel;
		/// <summary>
		///  公会ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会ID", " 公会ID")]
		public UInt64 guildId;
		/// <summary>
		///  公会名
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会名", " 公会名")]
		public string guildName;
		/// <summary>
		///  玩家标签信息
		/// </summary>
		[AdvancedInspector.Descriptor(" 玩家标签信息", " 玩家标签信息")]
		public PlayerLabelInfo playerLabelInfo = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, inviterId);
			byte[] inviterNameBytes = StringHelper.StringToUTF8Bytes(inviterName);
			BaseDLL.encode_string(buffer, ref pos_, inviterNameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_int8(buffer, ref pos_, inviterOccu);
			BaseDLL.encode_uint16(buffer, ref pos_, inviterLevel);
			BaseDLL.encode_int8(buffer, ref pos_, inviterVipLevel);
			BaseDLL.encode_uint64(buffer, ref pos_, guildId);
			byte[] guildNameBytes = StringHelper.StringToUTF8Bytes(guildName);
			BaseDLL.encode_string(buffer, ref pos_, guildNameBytes, (UInt16)(buffer.Length - pos_));
			playerLabelInfo.encode(buffer, ref pos_);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref inviterId);
			UInt16 inviterNameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref inviterNameLen);
			byte[] inviterNameBytes = new byte[inviterNameLen];
			for(int i = 0; i < inviterNameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref inviterNameBytes[i]);
			}
			inviterName = StringHelper.BytesToString(inviterNameBytes);
			BaseDLL.decode_int8(buffer, ref pos_, ref inviterOccu);
			BaseDLL.decode_uint16(buffer, ref pos_, ref inviterLevel);
			BaseDLL.decode_int8(buffer, ref pos_, ref inviterVipLevel);
			BaseDLL.decode_uint64(buffer, ref pos_, ref guildId);
			UInt16 guildNameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref guildNameLen);
			byte[] guildNameBytes = new byte[guildNameLen];
			for(int i = 0; i < guildNameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref guildNameBytes[i]);
			}
			guildName = StringHelper.BytesToString(guildNameBytes);
			playerLabelInfo.decode(buffer, ref pos_);
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
