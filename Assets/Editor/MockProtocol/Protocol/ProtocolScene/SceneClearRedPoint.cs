using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  清除公会红点
	/// </summary>
	[AdvancedInspector.Descriptor(" 清除公会红点", " 清除公会红点")]
	public class SceneClearRedPoint : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 500617;
		public UInt32 Sequence;
		/// <summary>
		///  红点类型（对应枚举RedPointFlag）
		/// </summary>
		[AdvancedInspector.Descriptor(" 红点类型（对应枚举RedPointFlag）", " 红点类型（对应枚举RedPointFlag）")]
		public UInt32 flag;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, flag);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref flag);
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
