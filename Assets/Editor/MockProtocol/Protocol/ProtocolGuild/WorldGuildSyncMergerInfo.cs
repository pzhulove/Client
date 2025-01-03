using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// world -> client  同步公会兼并相关信息
	/// </summary>
	[AdvancedInspector.Descriptor("world -> client  同步公会兼并相关信息", "world -> client  同步公会兼并相关信息")]
	public class WorldGuildSyncMergerInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601987;
		public UInt32 Sequence;
		/// <summary>
		/// 公会繁荣状态 1解散 2低繁荣 3中繁荣 4高繁荣
		/// </summary>
		[AdvancedInspector.Descriptor("公会繁荣状态 1解散 2低繁荣 3中繁荣 4高繁荣", "公会繁荣状态 1解散 2低繁荣 3中繁荣 4高繁荣")]
		public byte prosperityStatus;
		/// <summary>
		/// 请求状态 0无请求 1已申请 2已接受
		/// </summary>
		[AdvancedInspector.Descriptor("请求状态 0无请求 1已申请 2已接受", "请求状态 0无请求 1已申请 2已接受")]
		public byte mergerRequsetStatus;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, prosperityStatus);
			BaseDLL.encode_int8(buffer, ref pos_, mergerRequsetStatus);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref prosperityStatus);
			BaseDLL.decode_int8(buffer, ref pos_, ref mergerRequsetStatus);
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
