using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  请求死亡之塔首通奖励
	/// </summary>
	[AdvancedInspector.Descriptor(" 请求死亡之塔首通奖励", " 请求死亡之塔首通奖励")]
	public class SceneTowerFloorAwardReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 507209;
		public UInt32 Sequence;

		public UInt32 floor;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, floor);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref floor);
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
