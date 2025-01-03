using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 暴击倍数
	/// </summary>
	/// <summary>
	/// 罐子积分请求
	/// </summary>
	[AdvancedInspector.Descriptor("罐子积分请求", "罐子积分请求")]
	public class SceneJarPointReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 500960;
		public UInt32 Sequence;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
		}

		public void decode(byte[] buffer, ref int pos_)
		{
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
