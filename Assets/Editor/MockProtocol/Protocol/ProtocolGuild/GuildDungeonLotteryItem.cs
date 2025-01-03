using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  公会地下城奖励
	/// </summary>
	[AdvancedInspector.Descriptor(" 公会地下城奖励", " 公会地下城奖励")]
	public class GuildDungeonLotteryItem : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt32 itemId;

		public UInt32 itemNum;

		public UInt32 isHighVal;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, itemId);
			BaseDLL.encode_uint32(buffer, ref pos_, itemNum);
			BaseDLL.encode_uint32(buffer, ref pos_, isHighVal);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref itemId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref itemNum);
			BaseDLL.decode_uint32(buffer, ref pos_, ref isHighVal);
		}


		#endregion

	}

}
