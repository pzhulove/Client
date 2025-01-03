using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 同步16强赛单组状态
	/// </summary>
	[AdvancedInspector.Descriptor("同步16强赛单组状态", "同步16强赛单组状态")]
	public class SceneChampionGroupStatusSync : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 509831;
		public UInt32 Sequence;

		public ChampionGroup group = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			group.encode(buffer, ref pos_);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			group.decode(buffer, ref pos_);
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
