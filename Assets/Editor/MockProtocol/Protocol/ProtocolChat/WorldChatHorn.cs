using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  广播喇叭给客户端
	/// </summary>
	[AdvancedInspector.Descriptor(" 广播喇叭给客户端", " 广播喇叭给客户端")]
	public class WorldChatHorn : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 600815;
		public UInt32 Sequence;
		/// <summary>
		///  喇叭信息
		/// </summary>
		[AdvancedInspector.Descriptor(" 喇叭信息", " 喇叭信息")]
		public HornInfo info = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			info.encode(buffer, ref pos_);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			info.decode(buffer, ref pos_);
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
