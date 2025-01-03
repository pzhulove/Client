using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  请求对战记录
	/// </summary>
	[AdvancedInspector.Descriptor(" 请求对战记录", " 请求对战记录")]
	public class SceneReplayListReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 507501;
		public UInt32 Sequence;
		/// <summary>
		///  录像列表类型（对应枚举ReplayListType）
		/// </summary>
		[AdvancedInspector.Descriptor(" 录像列表类型（对应枚举ReplayListType）", " 录像列表类型（对应枚举ReplayListType）")]
		public byte type;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, type);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref type);
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
