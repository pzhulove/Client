using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 邮件标题信息
	/// </summary>
	[AdvancedInspector.Descriptor("邮件标题信息", "邮件标题信息")]
	public class MailTitleInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		/// 邮件ID
		/// </summary>
		[AdvancedInspector.Descriptor("邮件ID", "邮件ID")]
		public UInt64 id;
		/// <summary>
		/// 发件人
		/// </summary>
		[AdvancedInspector.Descriptor("发件人", "发件人")]
		public string sender;
		/// <summary>
		/// 发送日期
		/// </summary>
		[AdvancedInspector.Descriptor("发送日期", "发送日期")]
		public UInt32 date;
		/// <summary>
		/// 截至日期
		/// </summary>
		[AdvancedInspector.Descriptor("截至日期", "截至日期")]
		public UInt32 deadline;
		/// <summary>
		/// 邮件类型
		/// </summary>
		[AdvancedInspector.Descriptor("邮件类型", "邮件类型")]
		public byte type;
		/// <summary>
		/// 状态	0未读 1已读
		/// </summary>
		[AdvancedInspector.Descriptor("状态	0未读 1已读", "状态	0未读 1已读")]
		public byte status;
		/// <summary>
		/// 是否有附件 0没有 1有
		/// </summary>
		[AdvancedInspector.Descriptor("是否有附件 0没有 1有", "是否有附件 0没有 1有")]
		public byte hasItem;
		/// <summary>
		/// 标题
		/// </summary>
		[AdvancedInspector.Descriptor("标题", "标题")]
		public string title;
		/// <summary>
		/// 物品数据ID,用于显示图标
		/// </summary>
		[AdvancedInspector.Descriptor("物品数据ID,用于显示图标", "物品数据ID,用于显示图标")]
		public UInt32 itemId;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, id);
			byte[] senderBytes = StringHelper.StringToUTF8Bytes(sender);
			BaseDLL.encode_string(buffer, ref pos_, senderBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_uint32(buffer, ref pos_, date);
			BaseDLL.encode_uint32(buffer, ref pos_, deadline);
			BaseDLL.encode_int8(buffer, ref pos_, type);
			BaseDLL.encode_int8(buffer, ref pos_, status);
			BaseDLL.encode_int8(buffer, ref pos_, hasItem);
			byte[] titleBytes = StringHelper.StringToUTF8Bytes(title);
			BaseDLL.encode_string(buffer, ref pos_, titleBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_uint32(buffer, ref pos_, itemId);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref id);
			UInt16 senderLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref senderLen);
			byte[] senderBytes = new byte[senderLen];
			for(int i = 0; i < senderLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref senderBytes[i]);
			}
			sender = StringHelper.BytesToString(senderBytes);
			BaseDLL.decode_uint32(buffer, ref pos_, ref date);
			BaseDLL.decode_uint32(buffer, ref pos_, ref deadline);
			BaseDLL.decode_int8(buffer, ref pos_, ref type);
			BaseDLL.decode_int8(buffer, ref pos_, ref status);
			BaseDLL.decode_int8(buffer, ref pos_, ref hasItem);
			UInt16 titleLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref titleLen);
			byte[] titleBytes = new byte[titleLen];
			for(int i = 0; i < titleLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref titleBytes[i]);
			}
			title = StringHelper.BytesToString(titleBytes);
			BaseDLL.decode_uint32(buffer, ref pos_, ref itemId);
		}


		#endregion

	}

}
