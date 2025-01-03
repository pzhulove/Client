using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// scene->client 异常交易记录返回
	/// </summary>
	[AdvancedInspector.Descriptor("scene->client 异常交易记录返回", "scene->client 异常交易记录返回")]
	public class SceneAbnormalTransactionRecordRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 503907;
		public UInt32 Sequence;
		/// <summary>
		///  冻结金类型
		/// </summary>
		[AdvancedInspector.Descriptor(" 冻结金类型", " 冻结金类型")]
		public UInt32 frozenMoneyType;
		/// <summary>
		///  冻结金额
		/// </summary>
		[AdvancedInspector.Descriptor(" 冻结金额", " 冻结金额")]
		public UInt32 frozenAmount;
		/// <summary>
		///  异常交易时间
		/// </summary>
		[AdvancedInspector.Descriptor(" 异常交易时间", " 异常交易时间")]
		public UInt32 abnormalTransactionTime;
		/// <summary>
		///  返还期
		/// </summary>
		[AdvancedInspector.Descriptor(" 返还期", " 返还期")]
		public UInt32 backDeadline;
		/// <summary>
		///  已返还金额
		/// </summary>
		[AdvancedInspector.Descriptor(" 已返还金额", " 已返还金额")]
		public UInt32 backAmount;
		/// <summary>
		///  已返还日
		/// </summary>
		[AdvancedInspector.Descriptor(" 已返还日", " 已返还日")]
		public UInt32 backDays;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, frozenMoneyType);
			BaseDLL.encode_uint32(buffer, ref pos_, frozenAmount);
			BaseDLL.encode_uint32(buffer, ref pos_, abnormalTransactionTime);
			BaseDLL.encode_uint32(buffer, ref pos_, backDeadline);
			BaseDLL.encode_uint32(buffer, ref pos_, backAmount);
			BaseDLL.encode_uint32(buffer, ref pos_, backDays);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref frozenMoneyType);
			BaseDLL.decode_uint32(buffer, ref pos_, ref frozenAmount);
			BaseDLL.decode_uint32(buffer, ref pos_, ref abnormalTransactionTime);
			BaseDLL.decode_uint32(buffer, ref pos_, ref backDeadline);
			BaseDLL.decode_uint32(buffer, ref pos_, ref backAmount);
			BaseDLL.decode_uint32(buffer, ref pos_, ref backDays);
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
