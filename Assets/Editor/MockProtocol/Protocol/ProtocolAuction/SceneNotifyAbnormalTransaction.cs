using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// scene->client 通知异常交易
	/// </summary>
	[AdvancedInspector.Descriptor("scene->client 通知异常交易", "scene->client 通知异常交易")]
	public class SceneNotifyAbnormalTransaction : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 503905;
		public UInt32 Sequence;
		/// <summary>
		///  bool值(false(0):关闭通知，true(1):开启通知)
		/// </summary>
		[AdvancedInspector.Descriptor(" bool值(false(0):关闭通知，true(1):开启通知)", " bool值(false(0):关闭通知，true(1):开启通知)")]
		public byte bNotify;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, bNotify);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref bNotify);
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
