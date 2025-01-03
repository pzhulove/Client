using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  团本重连通知
	/// </summary>
	[AdvancedInspector.Descriptor(" 团本重连通知", " 团本重连通知")]
	public class TeamCopyReconnectNotify : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 1100040;
		public UInt32 Sequence;
		/// <summary>
		///  重连返回场景
		/// </summary>
		[AdvancedInspector.Descriptor(" 重连返回场景", " 重连返回场景")]
		public UInt32 sceneId;
		/// <summary>
		///  玩家过期时间
		/// </summary>
		[AdvancedInspector.Descriptor(" 玩家过期时间", " 玩家过期时间")]
		public UInt64 expireTime;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, sceneId);
			BaseDLL.encode_uint64(buffer, ref pos_, expireTime);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref sceneId);
			BaseDLL.decode_uint64(buffer, ref pos_, ref expireTime);
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
