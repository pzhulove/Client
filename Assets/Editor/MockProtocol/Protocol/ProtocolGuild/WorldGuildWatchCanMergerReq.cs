using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// client->world 查看可兼并的公会列表请求
	/// </summary>
	[AdvancedInspector.Descriptor("client->world 查看可兼并的公会列表请求", "client->world 查看可兼并的公会列表请求")]
	public class WorldGuildWatchCanMergerReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601977;
		public UInt32 Sequence;
		/// <summary>
		///  开始位置 0开始
		/// </summary>
		[AdvancedInspector.Descriptor(" 开始位置 0开始", " 开始位置 0开始")]
		public UInt16 start;
		/// <summary>
		///  数量
		/// </summary>
		[AdvancedInspector.Descriptor(" 数量", " 数量")]
		public UInt16 num;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, start);
			BaseDLL.encode_uint16(buffer, ref pos_, num);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint16(buffer, ref pos_, ref start);
			BaseDLL.decode_uint16(buffer, ref pos_, ref num);
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
