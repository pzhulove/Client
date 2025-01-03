using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 宝箱选择道具请求
	/// </summary>
	[AdvancedInspector.Descriptor("宝箱选择道具请求", "宝箱选择道具请求")]
	public class SceneLostDungeonBoxChoiceItemReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 510022;
		public UInt32 Sequence;

		public UInt32 itemId;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, itemId);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref itemId);
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
