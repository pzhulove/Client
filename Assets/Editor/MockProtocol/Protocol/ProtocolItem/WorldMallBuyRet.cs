using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 购买数量
	/// </summary>
	/// <summary>
	/// 购买商城道具返回
	/// </summary>
	[AdvancedInspector.Descriptor("购买商城道具返回", "购买商城道具返回")]
	public class WorldMallBuyRet : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 602802;
		public UInt32 Sequence;

		public UInt32 code;
		/// <summary>
		/// 返回码
		/// </summary>
		[AdvancedInspector.Descriptor("返回码", "返回码")]
		public UInt32 mallitemid;
		/// <summary>
		/// 商城道具id
		/// </summary>
		[AdvancedInspector.Descriptor("商城道具id", "商城道具id")]
		public Int32 restLimitNum;
		/// <summary>
		/// 剩余限购数,-1是没有限购
		/// </summary>
		/// <summary>
		/// 账号剩余购买次数
		/// </summary>
		[AdvancedInspector.Descriptor("账号剩余购买次数", "账号剩余购买次数")]
		public UInt32 accountRestBuyNum;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, code);
			BaseDLL.encode_uint32(buffer, ref pos_, mallitemid);
			BaseDLL.encode_int32(buffer, ref pos_, restLimitNum);
			BaseDLL.encode_uint32(buffer, ref pos_, accountRestBuyNum);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			BaseDLL.decode_uint32(buffer, ref pos_, ref mallitemid);
			BaseDLL.decode_int32(buffer, ref pos_, ref restLimitNum);
			BaseDLL.decode_uint32(buffer, ref pos_, ref accountRestBuyNum);
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
