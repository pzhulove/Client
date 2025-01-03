using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 客户端通知赛季播放状态
	/// </summary>
	[AdvancedInspector.Descriptor("客户端通知赛季播放状态", "客户端通知赛季播放状态")]
	public class SceneSeasonPlayStatus : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 506712;
		public UInt32 Sequence;

		public UInt32 seasonId;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, seasonId);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref seasonId);
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
