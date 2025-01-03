using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// scene->client 同步迷失战场数据
	/// </summary>
	[AdvancedInspector.Descriptor("scene->client 同步迷失战场数据", "scene->client 同步迷失战场数据")]
	public class BattleLostDungSyncBattleData : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 510029;
		public UInt32 Sequence;

		public UInt32 battleId;

		public UInt32 playerNum;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, battleId);
			BaseDLL.encode_uint32(buffer, ref pos_, playerNum);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref battleId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref playerNum);
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
