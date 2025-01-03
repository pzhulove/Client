using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  client->scene  选择地下城挑战模式请求
	/// </summary>
	[AdvancedInspector.Descriptor(" client->scene  选择地下城挑战模式请求", " client->scene  选择地下城挑战模式请求")]
	public class SceneLostDungeonSwitchChageModeReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 510008;
		public UInt32 Sequence;

		public byte chageMode;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, chageMode);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref chageMode);
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
