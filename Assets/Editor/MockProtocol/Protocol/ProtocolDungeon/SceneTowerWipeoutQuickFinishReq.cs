using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  请求快速完成死亡之塔扫荡
	/// </summary>
	[AdvancedInspector.Descriptor(" 请求快速完成死亡之塔扫荡", " 请求快速完成死亡之塔扫荡")]
	public class SceneTowerWipeoutQuickFinishReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 507205;
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
