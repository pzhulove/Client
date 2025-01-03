using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// Union->Clinet 战斗开始结束时间同步(用于客户端倒计时)
	/// </summary>
	[AdvancedInspector.Descriptor("Union->Clinet 战斗开始结束时间同步(用于客户端倒计时)", "Union->Clinet 战斗开始结束时间同步(用于客户端倒计时)")]
	public class UnionChampionBattleCountdownSync : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 2709810;
		public UInt32 Sequence;

		public UInt32 endTime;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, endTime);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref endTime);
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
