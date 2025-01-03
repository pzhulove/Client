using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  请求自身公会排行响应
	/// </summary>
	[AdvancedInspector.Descriptor(" 请求自身公会排行响应", " 请求自身公会排行响应")]
	public class WorldGuildBattleSelfSortListRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601956;
		public UInt32 Sequence;

		public UInt32 result;

		public UInt32 memberRanking;

		public UInt32 guildRanking;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, result);
			BaseDLL.encode_uint32(buffer, ref pos_, memberRanking);
			BaseDLL.encode_uint32(buffer, ref pos_, guildRanking);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			BaseDLL.decode_uint32(buffer, ref pos_, ref memberRanking);
			BaseDLL.decode_uint32(buffer, ref pos_, ref guildRanking);
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
