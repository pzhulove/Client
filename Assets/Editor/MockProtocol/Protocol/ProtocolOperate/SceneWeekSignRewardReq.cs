using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  周签到奖励领取请求
	/// </summary>
	[AdvancedInspector.Descriptor(" 周签到奖励领取请求", " 周签到奖励领取请求")]
	public class SceneWeekSignRewardReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 507408;
		public UInt32 Sequence;
		/// <summary>
		///  活动类型
		/// </summary>
		[AdvancedInspector.Descriptor(" 活动类型", " 活动类型")]
		public UInt32 opActType;
		/// <summary>
		///  第几周
		/// </summary>
		[AdvancedInspector.Descriptor(" 第几周", " 第几周")]
		public UInt32 weekID;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, opActType);
			BaseDLL.encode_uint32(buffer, ref pos_, weekID);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref opActType);
			BaseDLL.decode_uint32(buffer, ref pos_, ref weekID);
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
