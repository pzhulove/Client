using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// Scene->Client 请求冠军赛状态返回
	/// </summary>
	[AdvancedInspector.Descriptor("Scene->Client 请求冠军赛状态返回", "Scene->Client 请求冠军赛状态返回")]
	public class SceneChampionStatusRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 509818;
		public UInt32 Sequence;
		/// <summary>
		/// ChampionStatus
		/// </summary>
		[AdvancedInspector.Descriptor("ChampionStatus", "ChampionStatus")]
		public ChampionStatusInfo status = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			status.encode(buffer, ref pos_);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			status.decode(buffer, ref pos_);
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
