using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  通知客户端获得经验
	/// </summary>
	[AdvancedInspector.Descriptor(" 通知客户端获得经验", " 通知客户端获得经验")]
	public class SceneNotifyIncExp : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 500632;
		public UInt32 Sequence;
		/// <summary>
		///  获取经验来源(对应枚举PlayerIncExpReason)
		/// </summary>
		[AdvancedInspector.Descriptor(" 获取经验来源(对应枚举PlayerIncExpReason)", " 获取经验来源(对应枚举PlayerIncExpReason)")]
		public byte reason;
		/// <summary>
		///  获得的值(可能是数值，可能是百分比，根据来源决定)
		/// </summary>
		[AdvancedInspector.Descriptor(" 获得的值(可能是数值，可能是百分比，根据来源决定)", " 获得的值(可能是数值，可能是百分比，根据来源决定)")]
		public UInt32 value;
		/// <summary>
		///  获得的经验值
		/// </summary>
		[AdvancedInspector.Descriptor(" 获得的经验值", " 获得的经验值")]
		public UInt32 incExp;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, reason);
			BaseDLL.encode_uint32(buffer, ref pos_, value);
			BaseDLL.encode_uint32(buffer, ref pos_, incExp);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref reason);
			BaseDLL.decode_uint32(buffer, ref pos_, ref value);
			BaseDLL.decode_uint32(buffer, ref pos_, ref incExp);
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
