using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 罐子积分请求响应
	/// </summary>
	[AdvancedInspector.Descriptor("罐子积分请求响应", "罐子积分请求响应")]
	public class SceneJarPointRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 500961;
		public UInt32 Sequence;

		public UInt32 goldPoint;

		public UInt32 magPoint;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, goldPoint);
			BaseDLL.encode_uint32(buffer, ref pos_, magPoint);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref goldPoint);
			BaseDLL.decode_uint32(buffer, ref pos_, ref magPoint);
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
