using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 神器罐子折扣信息同步
	/// </summary>
	[AdvancedInspector.Descriptor("神器罐子折扣信息同步", "神器罐子折扣信息同步")]
	public class SceneArtifactJarDiscountInfoSync : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 507403;
		public UInt32 Sequence;
		/// <summary>
		///  运营活动id
		/// </summary>
		[AdvancedInspector.Descriptor(" 运营活动id", " 运营活动id")]
		public UInt32 opActId;
		/// <summary>
		///  抽取折扣状态(ArtifactJarDiscountExtractStatus)
		/// </summary>
		[AdvancedInspector.Descriptor(" 抽取折扣状态(ArtifactJarDiscountExtractStatus)", " 抽取折扣状态(ArtifactJarDiscountExtractStatus)")]
		public byte extractDiscountStatus;
		/// <summary>
		///  折扣率
		/// </summary>
		[AdvancedInspector.Descriptor(" 折扣率", " 折扣率")]
		public UInt32 discountRate;
		/// <summary>
		///  折扣生效次数
		/// </summary>
		[AdvancedInspector.Descriptor(" 折扣生效次数", " 折扣生效次数")]
		public UInt32 discountEffectTimes;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, opActId);
			BaseDLL.encode_int8(buffer, ref pos_, extractDiscountStatus);
			BaseDLL.encode_uint32(buffer, ref pos_, discountRate);
			BaseDLL.encode_uint32(buffer, ref pos_, discountEffectTimes);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref opActId);
			BaseDLL.decode_int8(buffer, ref pos_, ref extractDiscountStatus);
			BaseDLL.decode_uint32(buffer, ref pos_, ref discountRate);
			BaseDLL.decode_uint32(buffer, ref pos_, ref discountEffectTimes);
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
