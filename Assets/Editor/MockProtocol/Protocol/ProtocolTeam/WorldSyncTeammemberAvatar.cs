using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  同步外观信息
	/// </summary>
	[AdvancedInspector.Descriptor(" 同步外观信息", " 同步外观信息")]
	public class WorldSyncTeammemberAvatar : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601636;
		public UInt32 Sequence;
		/// <summary>
		///  成员ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 成员ID", " 成员ID")]
		public UInt64 memberId;
		/// <summary>
		///  外观
		/// </summary>
		[AdvancedInspector.Descriptor(" 外观", " 外观")]
		public PlayerAvatar avatar = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, memberId);
			avatar.encode(buffer, ref pos_);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref memberId);
			avatar.decode(buffer, ref pos_);
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
