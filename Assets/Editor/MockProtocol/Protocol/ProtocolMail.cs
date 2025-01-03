using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  邮件类型
	/// </summary>
	[AdvancedInspector.Descriptor(" 邮件类型", " 邮件类型")]
	public enum MailType
	{
		/// <summary>
		///  系统邮件
		/// </summary>
		[AdvancedInspector.Descriptor(" 系统邮件", " 系统邮件")]
		MAIL_TYPE_SYSTEM = 0,
		/// <summary>
		///  普通邮件
		/// </summary>
		[AdvancedInspector.Descriptor(" 普通邮件", " 普通邮件")]
		MAIL_TYPE_NORMAL = 1,
		/// <summary>
		///  GM邮件
		/// </summary>
		[AdvancedInspector.Descriptor(" GM邮件", " GM邮件")]
		MAIL_TYPE_GM = 2,
		/// <summary>
		///  公会邮件
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会邮件", " 公会邮件")]
		MAIL_TYPE_GUILD = 3,
	}

}
