using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  翻牌奖励
	/// </summary>
	[AdvancedInspector.Descriptor(" 翻牌奖励", " 翻牌奖励")]
	public class TeamCopyFlop : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  玩家name
		/// </summary>
		[AdvancedInspector.Descriptor(" 玩家name", " 玩家name")]
		public string playerName;
		/// <summary>
		///  玩家id
		/// </summary>
		[AdvancedInspector.Descriptor(" 玩家id", " 玩家id")]
		public UInt64 playerId;
		/// <summary>
		///  奖励id
		/// </summary>
		[AdvancedInspector.Descriptor(" 奖励id", " 奖励id")]
		public UInt32 rewardId;
		/// <summary>
		///  奖励数量
		/// </summary>
		[AdvancedInspector.Descriptor(" 奖励数量", " 奖励数量")]
		public UInt32 rewardNum;
		/// <summary>
		///  序号
		/// </summary>
		[AdvancedInspector.Descriptor(" 序号", " 序号")]
		public UInt32 number;
		/// <summary>
		///  是否金牌
		/// </summary>
		[AdvancedInspector.Descriptor(" 是否金牌", " 是否金牌")]
		public UInt32 goldFlop;
		/// <summary>
		///  是否限制(TeamCopyFlopLimit)
		/// </summary>
		[AdvancedInspector.Descriptor(" 是否限制(TeamCopyFlopLimit)", " 是否限制(TeamCopyFlopLimit)")]
		public UInt32 isLimit;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			byte[] playerNameBytes = StringHelper.StringToUTF8Bytes(playerName);
			BaseDLL.encode_string(buffer, ref pos_, playerNameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_uint64(buffer, ref pos_, playerId);
			BaseDLL.encode_uint32(buffer, ref pos_, rewardId);
			BaseDLL.encode_uint32(buffer, ref pos_, rewardNum);
			BaseDLL.encode_uint32(buffer, ref pos_, number);
			BaseDLL.encode_uint32(buffer, ref pos_, goldFlop);
			BaseDLL.encode_uint32(buffer, ref pos_, isLimit);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 playerNameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref playerNameLen);
			byte[] playerNameBytes = new byte[playerNameLen];
			for(int i = 0; i < playerNameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref playerNameBytes[i]);
			}
			playerName = StringHelper.BytesToString(playerNameBytes);
			BaseDLL.decode_uint64(buffer, ref pos_, ref playerId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref rewardId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref rewardNum);
			BaseDLL.decode_uint32(buffer, ref pos_, ref number);
			BaseDLL.decode_uint32(buffer, ref pos_, ref goldFlop);
			BaseDLL.decode_uint32(buffer, ref pos_, ref isLimit);
		}


		#endregion

	}

}
