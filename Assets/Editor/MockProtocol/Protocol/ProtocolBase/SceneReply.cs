using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  答复
	/// </summary>
	[AdvancedInspector.Descriptor(" 答复", " 答复")]
	public class SceneReply : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 500806;
		public UInt32 Sequence;
		/// <summary>
		/// 类型(对应枚举RequestType)
		/// </summary>
		[AdvancedInspector.Descriptor("类型(对应枚举RequestType)", "类型(对应枚举RequestType)")]
		public byte type;
		/// <summary>
		/// 请求者
		/// </summary>
		[AdvancedInspector.Descriptor("请求者", "请求者")]
		public UInt64 requester;
		/// <summary>
		/// 结果	1为接收 0为拒接
		/// </summary>
		[AdvancedInspector.Descriptor("结果	1为接收 0为拒接", "结果	1为接收 0为拒接")]
		public byte result;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, type);
			BaseDLL.encode_uint64(buffer, ref pos_, requester);
			BaseDLL.encode_int8(buffer, ref pos_, result);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref type);
			BaseDLL.decode_uint64(buffer, ref pos_, ref requester);
			BaseDLL.decode_int8(buffer, ref pos_, ref result);
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
