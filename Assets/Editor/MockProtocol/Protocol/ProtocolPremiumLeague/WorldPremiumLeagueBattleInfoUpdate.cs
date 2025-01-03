using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  更新淘汰赛对战信息
	/// </summary>
	[AdvancedInspector.Descriptor(" 更新淘汰赛对战信息", " 更新淘汰赛对战信息")]
	public class WorldPremiumLeagueBattleInfoUpdate : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 607714;
		public UInt32 Sequence;

		public CLPremiumLeagueBattle battle = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			battle.encode(buffer, ref pos_);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			battle.decode(buffer, ref pos_);
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
