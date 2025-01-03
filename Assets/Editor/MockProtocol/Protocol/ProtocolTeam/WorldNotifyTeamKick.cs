using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  组队解散倒计时状态
	/// </summary>
	[AdvancedInspector.Descriptor(" 组队解散倒计时状态", " 组队解散倒计时状态")]
	public class WorldNotifyTeamKick : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601660;
		public UInt32 Sequence;
		/// <summary>
		///  非0是踢出时间 0是停止
		/// </summary>
		[AdvancedInspector.Descriptor(" 非0是踢出时间 0是停止", " 非0是踢出时间 0是停止")]
		public UInt64 endTime;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, endTime);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref endTime);
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
