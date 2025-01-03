using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 快速购买请求
	/// </summary>
	[AdvancedInspector.Descriptor("快速购买请求", "快速购买请求")]
	public class SceneQuickBuyReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 507101;
		public UInt32 Sequence;
		/// <summary>
		///  类型(对应枚举QuickBuyTargetType)
		/// </summary>
		[AdvancedInspector.Descriptor(" 类型(对应枚举QuickBuyTargetType)", " 类型(对应枚举QuickBuyTargetType)")]
		public byte type;
		/// <summary>
		///  参数
		/// </summary>
		[AdvancedInspector.Descriptor(" 参数", " 参数")]
		public UInt64 param1;

		public UInt64 param2;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, type);
			BaseDLL.encode_uint64(buffer, ref pos_, param1);
			BaseDLL.encode_uint64(buffer, ref pos_, param2);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref type);
			BaseDLL.decode_uint64(buffer, ref pos_, ref param1);
			BaseDLL.decode_uint64(buffer, ref pos_, ref param2);
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
