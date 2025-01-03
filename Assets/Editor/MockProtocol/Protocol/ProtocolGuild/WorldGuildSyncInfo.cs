using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  上线或新加入公会发送初始数据
	/// </summary>
	[AdvancedInspector.Descriptor(" 上线或新加入公会发送初始数据", " 上线或新加入公会发送初始数据")]
	public class WorldGuildSyncInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601918;
		public UInt32 Sequence;
		/// <summary>
		///  公会基础信息
		/// </summary>
		[AdvancedInspector.Descriptor(" 公会基础信息", " 公会基础信息")]
		public GuildBaseInfo info = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			info.encode(buffer, ref pos_);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			info.decode(buffer, ref pos_);
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
