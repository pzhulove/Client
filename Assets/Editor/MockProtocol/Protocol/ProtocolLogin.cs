using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  玩家登陆状态
	/// </summary>
	[AdvancedInspector.Descriptor(" 玩家登陆状态", " 玩家登陆状态")]
	public enum PlayerLoginStatus
	{

		PLS_NONE = 0,
		/// <summary>
		///  每天第一次登录
		/// </summary>
		[AdvancedInspector.Descriptor(" 每天第一次登录", " 每天第一次登录")]
		PLS_FIRST_LOGIN_DAILY = 1,
	}


	public enum AuthIDType
	{
		/// <summary>
		///  未实名
		/// </summary>
		[AdvancedInspector.Descriptor(" 未实名", " 未实名")]
		AUTH_NO_ID = 0,
		/// <summary>
		///  未成年
		/// </summary>
		[AdvancedInspector.Descriptor(" 未成年", " 未成年")]
		AUTH_NO_ADULT = 1,
		/// <summary>
		///  成年
		/// </summary>
		[AdvancedInspector.Descriptor(" 成年", " 成年")]
		AUTH_ADULT = 2,
	}


	public enum SysSwitchType
	{

		SST_NONE = 0,
	}

}
