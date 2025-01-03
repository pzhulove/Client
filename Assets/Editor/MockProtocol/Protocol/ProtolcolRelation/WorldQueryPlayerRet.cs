using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  返回玩家信息
	/// </summary>
	[AdvancedInspector.Descriptor(" 返回玩家信息", " 返回玩家信息")]
	public class WorldQueryPlayerRet : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601702;
		public UInt32 Sequence;
		/// <summary>
		///  查询类型(QueryPlayerType)
		/// </summary>
		[AdvancedInspector.Descriptor(" 查询类型(QueryPlayerType)", " 查询类型(QueryPlayerType)")]
		public UInt32 queryType;
		/// <summary>
		/// zone(跨服查询需填)
		/// </summary>
		[AdvancedInspector.Descriptor("zone(跨服查询需填)", "zone(跨服查询需填)")]
		public UInt32 zoneId;

		public PlayerWatchInfo info = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, queryType);
			BaseDLL.encode_uint32(buffer, ref pos_, zoneId);
			info.encode(buffer, ref pos_);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref queryType);
			BaseDLL.decode_uint32(buffer, ref pos_, ref zoneId);
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
