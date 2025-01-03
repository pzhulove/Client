using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  通知客户端loading情况
	/// </summary>
	[AdvancedInspector.Descriptor(" 通知客户端loading情况", " 通知客户端loading情况")]
	public class SceneNotifyLoadingInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 500117;
		public UInt32 Sequence;
		/// <summary>
		///  是否在loading中
		/// </summary>
		[AdvancedInspector.Descriptor(" 是否在loading中", " 是否在loading中")]
		public byte isLoading;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, isLoading);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref isLoading);
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
