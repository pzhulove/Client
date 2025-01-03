using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 查询新月签到情况返回
	/// </summary>
	[AdvancedInspector.Descriptor("查询新月签到情况返回", "查询新月签到情况返回")]
	public class SceneNewSignInQueryRet : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501164;
		public UInt32 Sequence;
		/// <summary>
		/// 签到记录按位存储
		/// </summary>
		[AdvancedInspector.Descriptor("签到记录按位存储", "签到记录按位存储")]
		public UInt32 signFlag;
		/// <summary>
		/// 已领取累计奖励的累计天数按位存储
		/// </summary>
		[AdvancedInspector.Descriptor("已领取累计奖励的累计天数按位存储", "已领取累计奖励的累计天数按位存储")]
		public UInt32 collectFlag;
		/// <summary>
		/// 剩余收费补签次数
		/// </summary>
		[AdvancedInspector.Descriptor("剩余收费补签次数", "剩余收费补签次数")]
		public byte noFree;
		/// <summary>
		/// 剩余免费补签次数
		/// </summary>
		[AdvancedInspector.Descriptor("剩余免费补签次数", "剩余免费补签次数")]
		public byte free;
		/// <summary>
		/// 剩余活跃度补签次数
		/// </summary>
		[AdvancedInspector.Descriptor("剩余活跃度补签次数", "剩余活跃度补签次数")]
		public byte activite;
		/// <summary>
		/// 活跃度已补签次数
		/// </summary>
		[AdvancedInspector.Descriptor("活跃度已补签次数", "活跃度已补签次数")]
		public byte activiteCount;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, signFlag);
			BaseDLL.encode_uint32(buffer, ref pos_, collectFlag);
			BaseDLL.encode_int8(buffer, ref pos_, noFree);
			BaseDLL.encode_int8(buffer, ref pos_, free);
			BaseDLL.encode_int8(buffer, ref pos_, activite);
			BaseDLL.encode_int8(buffer, ref pos_, activiteCount);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref signFlag);
			BaseDLL.decode_uint32(buffer, ref pos_, ref collectFlag);
			BaseDLL.decode_int8(buffer, ref pos_, ref noFree);
			BaseDLL.decode_int8(buffer, ref pos_, ref free);
			BaseDLL.decode_int8(buffer, ref pos_, ref activite);
			BaseDLL.decode_int8(buffer, ref pos_, ref activiteCount);
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
