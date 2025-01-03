using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  聊天标记
	/// </summary>
	[AdvancedInspector.Descriptor(" 聊天标记", " 聊天标记")]
	public enum ChatMask
	{
		/// <summary>
		///  红包信息
		/// </summary>
		[AdvancedInspector.Descriptor(" 红包信息", " 红包信息")]
		CHAT_MASK_RED_PACKET = 1,
		/// <summary>
		///  添加好友
		/// </summary>
		[AdvancedInspector.Descriptor(" 添加好友", " 添加好友")]
		CHAT_MASK_ADD_FRIEND = 2,
	}

	/// <summary>
	///  自定义日志上报类型
	/// </summary>
	[AdvancedInspector.Descriptor(" 自定义日志上报类型", " 自定义日志上报类型")]
	public enum CustomLogReportType
	{
		/// <summary>
		///  非法
		/// </summary>
		[AdvancedInspector.Descriptor(" 非法", " 非法")]
		CLRT_INVALID = 0,
		/// <summary>
		///  加入房间
		/// </summary>
		[AdvancedInspector.Descriptor(" 加入房间", " 加入房间")]
		CLRT_JOIN_VOICE_ROOM = 1,
		/// <summary>
		///  退出房间
		/// </summary>
		[AdvancedInspector.Descriptor(" 退出房间", " 退出房间")]
		CLRT_QUIT_VOICE_ROOM = 2,
		/// <summary>
		///  发送录音
		/// </summary>
		[AdvancedInspector.Descriptor(" 发送录音", " 发送录音")]
		CLRT_SEND_RECORD_VOICE = 3,
		/// <summary>
		///  下载录音
		/// </summary>
		[AdvancedInspector.Descriptor(" 下载录音", " 下载录音")]
		CLRT_LOAD_RECORD_VOICE = 4,
	}

}
