using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 吃鸡准备场景的人数
	/// </summary>
	[AdvancedInspector.Descriptor("吃鸡准备场景的人数", "吃鸡准备场景的人数")]
	public class BattleNotifyPrepareInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 2200009;
		public UInt32 Sequence;

		public UInt32 playerNum;

		public UInt32 totalNum;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, playerNum);
			BaseDLL.encode_uint32(buffer, ref pos_, totalNum);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref playerNum);
			BaseDLL.decode_uint32(buffer, ref pos_, ref totalNum);
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
