using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  返回玩家上报帧结果
	/// </summary>
	[AdvancedInspector.Descriptor(" 返回玩家上报帧结果", " 返回玩家上报帧结果")]
	public class WorldDungeonReportFrameRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 606812;
		public UInt32 Sequence;
		/// <summary>
		///  收到的最后一帧
		/// </summary>
		[AdvancedInspector.Descriptor(" 收到的最后一帧", " 收到的最后一帧")]
		public UInt32 lastFrame;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, lastFrame);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref lastFrame);
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
