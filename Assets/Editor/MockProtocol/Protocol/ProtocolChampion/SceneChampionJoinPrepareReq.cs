using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// Clinet->Scene 请求进入比赛准备区域
	/// </summary>
	[AdvancedInspector.Descriptor("Clinet->Scene 请求进入比赛准备区域", "Clinet->Scene 请求进入比赛准备区域")]
	public class SceneChampionJoinPrepareReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 509804;
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
