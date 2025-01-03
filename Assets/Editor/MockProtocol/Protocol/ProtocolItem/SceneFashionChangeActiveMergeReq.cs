using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  client->scene 换装节活动时装合成请求
	/// </summary>
	[AdvancedInspector.Descriptor(" client->scene 换装节活动时装合成请求", " client->scene 换装节活动时装合成请求")]
	public class SceneFashionChangeActiveMergeReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501029;
		public UInt32 Sequence;
		/// <summary>
		/// 被合时装对象id
		/// </summary>
		[AdvancedInspector.Descriptor("被合时装对象id", "被合时装对象id")]
		public UInt64 fashionId;
		/// <summary>
		/// 换装卷对象id（选择背包中数量最大的换装卷）
		/// </summary>
		[AdvancedInspector.Descriptor("换装卷对象id（选择背包中数量最大的换装卷）", "换装卷对象id（选择背包中数量最大的换装卷）")]
		public UInt64 ticketId;
		/// <summary>
		/// 选择必定合成时装道具id
		/// </summary>
		[AdvancedInspector.Descriptor("选择必定合成时装道具id", "选择必定合成时装道具id")]
		public UInt32 choiceComFashionId;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, fashionId);
			BaseDLL.encode_uint64(buffer, ref pos_, ticketId);
			BaseDLL.encode_uint32(buffer, ref pos_, choiceComFashionId);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref fashionId);
			BaseDLL.decode_uint64(buffer, ref pos_, ref ticketId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref choiceComFashionId);
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
