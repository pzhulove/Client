using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 赠送项信息
	/// </summary>
	[AdvancedInspector.Descriptor("赠送项信息", "赠送项信息")]
	public class FriendPresentInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  唯一id
		/// </summary>
		[AdvancedInspector.Descriptor(" 唯一id", " 唯一id")]
		public UInt64 guid;
		/// <summary>
		///  好友id
		/// </summary>
		[AdvancedInspector.Descriptor(" 好友id", " 好友id")]
		public UInt64 friendId;
		/// <summary>
		///  好友职业
		/// </summary>
		[AdvancedInspector.Descriptor(" 好友职业", " 好友职业")]
		public byte friendOcc;
		/// <summary>
		///  好友等级
		/// </summary>
		[AdvancedInspector.Descriptor(" 好友等级", " 好友等级")]
		public UInt16 friendLev;
		/// <summary>
		///  好友名字
		/// </summary>
		[AdvancedInspector.Descriptor(" 好友名字", " 好友名字")]
		public string friendname;
		/// <summary>
		///  在线状态
		/// </summary>
		[AdvancedInspector.Descriptor(" 在线状态", " 在线状态")]
		public byte isOnline;
		/// <summary>
		///  被赠送数量
		/// </summary>
		[AdvancedInspector.Descriptor(" 被赠送数量", " 被赠送数量")]
		public UInt32 beSendedTimes;
		/// <summary>
		///  被赠送上限
		/// </summary>
		[AdvancedInspector.Descriptor(" 被赠送上限", " 被赠送上限")]
		public UInt32 beSendedLimit;
		/// <summary>
		///  赠送数量
		/// </summary>
		[AdvancedInspector.Descriptor(" 赠送数量", " 赠送数量")]
		public UInt32 sendTimes;
		/// <summary>
		///  赠送上限
		/// </summary>
		[AdvancedInspector.Descriptor(" 赠送上限", " 赠送上限")]
		public UInt32 sendLimit;
		/// <summary>
		///  被赠送总次数
		/// </summary>
		[AdvancedInspector.Descriptor(" 被赠送总次数", " 被赠送总次数")]
		public UInt32 sendedTotalTimes;
		/// <summary>
		///  被赠送总次数上限
		/// </summary>
		[AdvancedInspector.Descriptor(" 被赠送总次数上限", " 被赠送总次数上限")]
		public UInt32 sendedTotalLimit;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, guid);
			BaseDLL.encode_uint64(buffer, ref pos_, friendId);
			BaseDLL.encode_int8(buffer, ref pos_, friendOcc);
			BaseDLL.encode_uint16(buffer, ref pos_, friendLev);
			byte[] friendnameBytes = StringHelper.StringToUTF8Bytes(friendname);
			BaseDLL.encode_string(buffer, ref pos_, friendnameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_int8(buffer, ref pos_, isOnline);
			BaseDLL.encode_uint32(buffer, ref pos_, beSendedTimes);
			BaseDLL.encode_uint32(buffer, ref pos_, beSendedLimit);
			BaseDLL.encode_uint32(buffer, ref pos_, sendTimes);
			BaseDLL.encode_uint32(buffer, ref pos_, sendLimit);
			BaseDLL.encode_uint32(buffer, ref pos_, sendedTotalTimes);
			BaseDLL.encode_uint32(buffer, ref pos_, sendedTotalLimit);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref guid);
			BaseDLL.decode_uint64(buffer, ref pos_, ref friendId);
			BaseDLL.decode_int8(buffer, ref pos_, ref friendOcc);
			BaseDLL.decode_uint16(buffer, ref pos_, ref friendLev);
			UInt16 friendnameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref friendnameLen);
			byte[] friendnameBytes = new byte[friendnameLen];
			for(int i = 0; i < friendnameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref friendnameBytes[i]);
			}
			friendname = StringHelper.BytesToString(friendnameBytes);
			BaseDLL.decode_int8(buffer, ref pos_, ref isOnline);
			BaseDLL.decode_uint32(buffer, ref pos_, ref beSendedTimes);
			BaseDLL.decode_uint32(buffer, ref pos_, ref beSendedLimit);
			BaseDLL.decode_uint32(buffer, ref pos_, ref sendTimes);
			BaseDLL.decode_uint32(buffer, ref pos_, ref sendLimit);
			BaseDLL.decode_uint32(buffer, ref pos_, ref sendedTotalTimes);
			BaseDLL.decode_uint32(buffer, ref pos_, ref sendedTotalLimit);
		}


		#endregion

	}

}
