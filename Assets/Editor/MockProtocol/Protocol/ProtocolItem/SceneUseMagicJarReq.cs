using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 开罐子
	/// </summary>
	[AdvancedInspector.Descriptor("开罐子", "开罐子")]
	public class SceneUseMagicJarReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 500948;
		public UInt32 Sequence;
		/// <summary>
		/// 开罐类型
		/// </summary>
		[AdvancedInspector.Descriptor("开罐类型", "开罐类型")]
		public UInt32 type;
		/// <summary>
		/// 是否连开
		/// </summary>
		[AdvancedInspector.Descriptor("是否连开", "是否连开")]
		public byte combo;
		/// <summary>
		///  运营活动id
		/// </summary>
		[AdvancedInspector.Descriptor(" 运营活动id", " 运营活动id")]
		public UInt32 opActId;
		/// <summary>
		///  参数
		/// </summary>
		[AdvancedInspector.Descriptor(" 参数", " 参数")]
		public UInt32 param;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, type);
			BaseDLL.encode_int8(buffer, ref pos_, combo);
			BaseDLL.encode_uint32(buffer, ref pos_, opActId);
			BaseDLL.encode_uint32(buffer, ref pos_, param);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref type);
			BaseDLL.decode_int8(buffer, ref pos_, ref combo);
			BaseDLL.decode_uint32(buffer, ref pos_, ref opActId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref param);
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
