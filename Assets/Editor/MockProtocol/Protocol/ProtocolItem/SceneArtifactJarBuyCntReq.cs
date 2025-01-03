using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  ************************神器罐子活动协议**************
	/// </summary>
	/// <summary>
	///  活动神器罐购买次数请求
	/// </summary>
	[AdvancedInspector.Descriptor(" 活动神器罐购买次数请求", " 活动神器罐购买次数请求")]
	public class SceneArtifactJarBuyCntReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501046;
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
