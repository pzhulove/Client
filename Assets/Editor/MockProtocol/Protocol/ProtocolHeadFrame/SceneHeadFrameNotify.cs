using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  头像框通知
	/// </summary>
	[AdvancedInspector.Descriptor(" 头像框通知", " 头像框通知")]
	public class SceneHeadFrameNotify : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 509105;
		public UInt32 Sequence;
		/// <summary>
		///  1是获得，0删除
		/// </summary>
		[AdvancedInspector.Descriptor(" 1是获得，0删除", " 1是获得，0删除")]
		public UInt32 isGet;
		/// <summary>
		///  头像框
		/// </summary>
		[AdvancedInspector.Descriptor(" 头像框", " 头像框")]
		public HeadFrame headFrame = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, isGet);
			headFrame.encode(buffer, ref pos_);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref isGet);
			headFrame.decode(buffer, ref pos_);
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
