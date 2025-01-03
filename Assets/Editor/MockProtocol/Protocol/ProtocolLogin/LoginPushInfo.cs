using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  登录推送信息
	/// </summary>
	[AdvancedInspector.Descriptor(" 登录推送信息", " 登录推送信息")]
	public class LoginPushInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  唯一id
		/// </summary>
		[AdvancedInspector.Descriptor(" 唯一id", " 唯一id")]
		public byte id;
		/// <summary>
		///  名称
		/// </summary>
		[AdvancedInspector.Descriptor(" 名称", " 名称")]
		public string name;
		/// <summary>
		///  解锁等级
		/// </summary>
		[AdvancedInspector.Descriptor(" 解锁等级", " 解锁等级")]
		public UInt16 unlockLevel;
		/// <summary>
		///  图标路径
		/// </summary>
		[AdvancedInspector.Descriptor(" 图标路径", " 图标路径")]
		public string iconPath;
		/// <summary>
		///  链接位置
		/// </summary>
		[AdvancedInspector.Descriptor(" 链接位置", " 链接位置")]
		public string linkInfo;
		/// <summary>
		///  开始时间
		/// </summary>
		[AdvancedInspector.Descriptor(" 开始时间", " 开始时间")]
		public UInt32 startTime;
		/// <summary>
		///  结束时间
		/// </summary>
		[AdvancedInspector.Descriptor(" 结束时间", " 结束时间")]
		public UInt32 endTime;
		/// <summary>
		///  loading预制体路径
		/// </summary>
		[AdvancedInspector.Descriptor(" loading预制体路径", " loading预制体路径")]
		public string loadingIconPath;
		/// <summary>
		///  排序序号
		/// </summary>
		[AdvancedInspector.Descriptor(" 排序序号", " 排序序号")]
		public byte sortNum;
		/// <summary>
		///  开启间隔
		/// </summary>
		[AdvancedInspector.Descriptor(" 开启间隔", " 开启间隔")]
		public string openInterval;
		/// <summary>
		///  关闭间隔
		/// </summary>
		[AdvancedInspector.Descriptor(" 关闭间隔", " 关闭间隔")]
		public string closeInterval;
		/// <summary>
		///  是否显示时间
		/// </summary>
		[AdvancedInspector.Descriptor(" 是否显示时间", " 是否显示时间")]
		public byte isShowTime;
		/// <summary>
		///  是否设置背景图片原比例大小
		/// </summary>
		[AdvancedInspector.Descriptor(" 是否设置背景图片原比例大小", " 是否设置背景图片原比例大小")]
		public byte isSetNative;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, id);
			byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
			BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_uint16(buffer, ref pos_, unlockLevel);
			byte[] iconPathBytes = StringHelper.StringToUTF8Bytes(iconPath);
			BaseDLL.encode_string(buffer, ref pos_, iconPathBytes, (UInt16)(buffer.Length - pos_));
			byte[] linkInfoBytes = StringHelper.StringToUTF8Bytes(linkInfo);
			BaseDLL.encode_string(buffer, ref pos_, linkInfoBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_uint32(buffer, ref pos_, startTime);
			BaseDLL.encode_uint32(buffer, ref pos_, endTime);
			byte[] loadingIconPathBytes = StringHelper.StringToUTF8Bytes(loadingIconPath);
			BaseDLL.encode_string(buffer, ref pos_, loadingIconPathBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_int8(buffer, ref pos_, sortNum);
			byte[] openIntervalBytes = StringHelper.StringToUTF8Bytes(openInterval);
			BaseDLL.encode_string(buffer, ref pos_, openIntervalBytes, (UInt16)(buffer.Length - pos_));
			byte[] closeIntervalBytes = StringHelper.StringToUTF8Bytes(closeInterval);
			BaseDLL.encode_string(buffer, ref pos_, closeIntervalBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_int8(buffer, ref pos_, isShowTime);
			BaseDLL.encode_int8(buffer, ref pos_, isSetNative);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref id);
			UInt16 nameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
			byte[] nameBytes = new byte[nameLen];
			for(int i = 0; i < nameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
			}
			name = StringHelper.BytesToString(nameBytes);
			BaseDLL.decode_uint16(buffer, ref pos_, ref unlockLevel);
			UInt16 iconPathLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref iconPathLen);
			byte[] iconPathBytes = new byte[iconPathLen];
			for(int i = 0; i < iconPathLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref iconPathBytes[i]);
			}
			iconPath = StringHelper.BytesToString(iconPathBytes);
			UInt16 linkInfoLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref linkInfoLen);
			byte[] linkInfoBytes = new byte[linkInfoLen];
			for(int i = 0; i < linkInfoLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref linkInfoBytes[i]);
			}
			linkInfo = StringHelper.BytesToString(linkInfoBytes);
			BaseDLL.decode_uint32(buffer, ref pos_, ref startTime);
			BaseDLL.decode_uint32(buffer, ref pos_, ref endTime);
			UInt16 loadingIconPathLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref loadingIconPathLen);
			byte[] loadingIconPathBytes = new byte[loadingIconPathLen];
			for(int i = 0; i < loadingIconPathLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref loadingIconPathBytes[i]);
			}
			loadingIconPath = StringHelper.BytesToString(loadingIconPathBytes);
			BaseDLL.decode_int8(buffer, ref pos_, ref sortNum);
			UInt16 openIntervalLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref openIntervalLen);
			byte[] openIntervalBytes = new byte[openIntervalLen];
			for(int i = 0; i < openIntervalLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref openIntervalBytes[i]);
			}
			openInterval = StringHelper.BytesToString(openIntervalBytes);
			UInt16 closeIntervalLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref closeIntervalLen);
			byte[] closeIntervalBytes = new byte[closeIntervalLen];
			for(int i = 0; i < closeIntervalLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref closeIntervalBytes[i]);
			}
			closeInterval = StringHelper.BytesToString(closeIntervalBytes);
			BaseDLL.decode_int8(buffer, ref pos_, ref isShowTime);
			BaseDLL.decode_int8(buffer, ref pos_, ref isSetNative);
		}


		#endregion

	}

}
