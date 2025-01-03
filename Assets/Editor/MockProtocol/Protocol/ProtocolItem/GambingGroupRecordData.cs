using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 夺宝组记录
	/// </summary>
	[AdvancedInspector.Descriptor("夺宝组记录", "夺宝组记录")]
	public class GambingGroupRecordData : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  组id
		/// </summary>
		[AdvancedInspector.Descriptor(" 组id", " 组id")]
		public UInt16 groupId;
		/// <summary>
		///  赢家平台(英文)
		/// </summary>
		[AdvancedInspector.Descriptor(" 赢家平台(英文)", " 赢家平台(英文)")]
		public string gainerENPlatform;
		/// <summary>
		///  赢家平台
		/// </summary>
		[AdvancedInspector.Descriptor(" 赢家平台", " 赢家平台")]
		public string gainerPlatform;
		/// <summary>
		///  赢家服务器
		/// </summary>
		[AdvancedInspector.Descriptor(" 赢家服务器", " 赢家服务器")]
		public string gainerServerName;
		/// <summary>
		///  赢家id
		/// </summary>
		[AdvancedInspector.Descriptor(" 赢家id", " 赢家id")]
		public UInt64 gainerId;
		/// <summary>
		///  赢家名字
		/// </summary>
		[AdvancedInspector.Descriptor(" 赢家名字", " 赢家名字")]
		public string gainerName;
		/// <summary>
		///  赢家投入货币id
		/// </summary>
		[AdvancedInspector.Descriptor(" 赢家投入货币id", " 赢家投入货币id")]
		public UInt32 investCurrencyId;
		/// <summary>
		///  赢家投入货币数量
		/// </summary>
		[AdvancedInspector.Descriptor(" 赢家投入货币数量", " 赢家投入货币数量")]
		public UInt32 investCurrencyNum;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, groupId);
			byte[] gainerENPlatformBytes = StringHelper.StringToUTF8Bytes(gainerENPlatform);
			BaseDLL.encode_string(buffer, ref pos_, gainerENPlatformBytes, (UInt16)(buffer.Length - pos_));
			byte[] gainerPlatformBytes = StringHelper.StringToUTF8Bytes(gainerPlatform);
			BaseDLL.encode_string(buffer, ref pos_, gainerPlatformBytes, (UInt16)(buffer.Length - pos_));
			byte[] gainerServerNameBytes = StringHelper.StringToUTF8Bytes(gainerServerName);
			BaseDLL.encode_string(buffer, ref pos_, gainerServerNameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_uint64(buffer, ref pos_, gainerId);
			byte[] gainerNameBytes = StringHelper.StringToUTF8Bytes(gainerName);
			BaseDLL.encode_string(buffer, ref pos_, gainerNameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_uint32(buffer, ref pos_, investCurrencyId);
			BaseDLL.encode_uint32(buffer, ref pos_, investCurrencyNum);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint16(buffer, ref pos_, ref groupId);
			UInt16 gainerENPlatformLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref gainerENPlatformLen);
			byte[] gainerENPlatformBytes = new byte[gainerENPlatformLen];
			for(int i = 0; i < gainerENPlatformLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref gainerENPlatformBytes[i]);
			}
			gainerENPlatform = StringHelper.BytesToString(gainerENPlatformBytes);
			UInt16 gainerPlatformLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref gainerPlatformLen);
			byte[] gainerPlatformBytes = new byte[gainerPlatformLen];
			for(int i = 0; i < gainerPlatformLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref gainerPlatformBytes[i]);
			}
			gainerPlatform = StringHelper.BytesToString(gainerPlatformBytes);
			UInt16 gainerServerNameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref gainerServerNameLen);
			byte[] gainerServerNameBytes = new byte[gainerServerNameLen];
			for(int i = 0; i < gainerServerNameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref gainerServerNameBytes[i]);
			}
			gainerServerName = StringHelper.BytesToString(gainerServerNameBytes);
			BaseDLL.decode_uint64(buffer, ref pos_, ref gainerId);
			UInt16 gainerNameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref gainerNameLen);
			byte[] gainerNameBytes = new byte[gainerNameLen];
			for(int i = 0; i < gainerNameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref gainerNameBytes[i]);
			}
			gainerName = StringHelper.BytesToString(gainerNameBytes);
			BaseDLL.decode_uint32(buffer, ref pos_, ref investCurrencyId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref investCurrencyNum);
		}


		#endregion

	}

}
