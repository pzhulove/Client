using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  server->client 同步冒险队信息
	/// </summary>
	[AdvancedInspector.Descriptor(" server->client 同步冒险队信息", " server->client 同步冒险队信息")]
	public class AdventureTeamInfoSync : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 308601;
		public UInt32 Sequence;
		/// <summary>
		///  冒险队信息
		/// </summary>
		[AdvancedInspector.Descriptor(" 冒险队信息", " 冒险队信息")]
		public AdventureTeamExtraInfo info = null;

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
