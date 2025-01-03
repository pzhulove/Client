using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  通知玩家组队快速匹配结果
	/// </summary>
	[AdvancedInspector.Descriptor(" 通知玩家组队快速匹配结果", " 通知玩家组队快速匹配结果")]
	public class WorldTeamMatchResultNotify : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601648;
		public UInt32 Sequence;
		/// <summary>
		///  地下城ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 地下城ID", " 地下城ID")]
		public UInt32 dungeonId;
		/// <summary>
		///  是否同意
		/// </summary>
		[AdvancedInspector.Descriptor(" 是否同意", " 是否同意")]
		public PlayerIcon[] players = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, dungeonId);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)players.Length);
			for(int i = 0; i < players.Length; i++)
			{
				players[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref dungeonId);
			UInt16 playersCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref playersCnt);
			players = new PlayerIcon[playersCnt];
			for(int i = 0; i < players.Length; i++)
			{
				players[i] = new PlayerIcon();
				players[i].decode(buffer, ref pos_);
			}
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
