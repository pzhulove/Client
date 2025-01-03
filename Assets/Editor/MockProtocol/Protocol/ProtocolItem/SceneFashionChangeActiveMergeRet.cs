using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  scene->client 换装节活动时装合成返回
	/// </summary>
	[AdvancedInspector.Descriptor(" scene->client 换装节活动时装合成返回", " scene->client 换装节活动时装合成返回")]
	public class SceneFashionChangeActiveMergeRet : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501030;
		public UInt32 Sequence;
		/// <summary>
		/// 结果
		/// </summary>
		[AdvancedInspector.Descriptor("结果", "结果")]
		public UInt32 ret;
		/// <summary>
		/// [1]:未合出进击的勇士时装 [2]合出进击的勇士时装
		/// </summary>
		[AdvancedInspector.Descriptor("[1]:未合出进击的勇士时装 [2]合出进击的勇士时装", "[1]:未合出进击的勇士时装 [2]合出进击的勇士时装")]
		public byte type;
		/// <summary>
		/// 合出普通时装id
		/// </summary>
		[AdvancedInspector.Descriptor("合出普通时装id", "合出普通时装id")]
		public UInt32 commonId;
		/// <summary>
		/// 合出的进击的勇士时装id
		/// </summary>
		[AdvancedInspector.Descriptor("合出的进击的勇士时装id", "合出的进击的勇士时装id")]
		public UInt32 advanceId;
		/// <summary>
		/// 套装全部合出
		/// </summary>
		[AdvancedInspector.Descriptor("套装全部合出", "套装全部合出")]
		public byte allMerged;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, ret);
			BaseDLL.encode_int8(buffer, ref pos_, type);
			BaseDLL.encode_uint32(buffer, ref pos_, commonId);
			BaseDLL.encode_uint32(buffer, ref pos_, advanceId);
			BaseDLL.encode_int8(buffer, ref pos_, allMerged);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref ret);
			BaseDLL.decode_int8(buffer, ref pos_, ref type);
			BaseDLL.decode_uint32(buffer, ref pos_, ref commonId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref advanceId);
			BaseDLL.decode_int8(buffer, ref pos_, ref allMerged);
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
