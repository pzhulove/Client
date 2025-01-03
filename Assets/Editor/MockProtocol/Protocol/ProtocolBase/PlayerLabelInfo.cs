using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  玩家标签信息
	/// </summary>
	[AdvancedInspector.Descriptor(" 玩家标签信息", " 玩家标签信息")]
	public class PlayerLabelInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  觉醒状态
		/// </summary>
		[AdvancedInspector.Descriptor(" 觉醒状态", " 觉醒状态")]
		public byte awakenStatus;
		/// <summary>
		///  回归状态
		/// </summary>
		[AdvancedInspector.Descriptor(" 回归状态", " 回归状态")]
		public byte returnStatus;
		/// <summary>
		///  新手引导选择标志
		/// </summary>
		[AdvancedInspector.Descriptor(" 新手引导选择标志", " 新手引导选择标志")]
		public byte noviceGuideChooseFlag;
		/// <summary>
		///  头像框
		/// </summary>
		[AdvancedInspector.Descriptor(" 头像框", " 头像框")]
		public UInt32 headFrame;
		/// <summary>
		///  公会ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会ID", " 公会ID")]
		public UInt64 guildId;
		/// <summary>
		///  回归周年称号
		/// </summary>
		[AdvancedInspector.Descriptor(" 回归周年称号", " 回归周年称号")]
		public UInt32 returnAnniversaryTitle;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, awakenStatus);
			BaseDLL.encode_int8(buffer, ref pos_, returnStatus);
			BaseDLL.encode_int8(buffer, ref pos_, noviceGuideChooseFlag);
			BaseDLL.encode_uint32(buffer, ref pos_, headFrame);
			BaseDLL.encode_uint64(buffer, ref pos_, guildId);
			BaseDLL.encode_uint32(buffer, ref pos_, returnAnniversaryTitle);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref awakenStatus);
			BaseDLL.decode_int8(buffer, ref pos_, ref returnStatus);
			BaseDLL.decode_int8(buffer, ref pos_, ref noviceGuideChooseFlag);
			BaseDLL.decode_uint32(buffer, ref pos_, ref headFrame);
			BaseDLL.decode_uint64(buffer, ref pos_, ref guildId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref returnAnniversaryTitle);
		}


		#endregion

	}

}
