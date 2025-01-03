using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  client->scene 地牢开宝箱请求
	/// </summary>
	[AdvancedInspector.Descriptor(" client->scene 地牢开宝箱请求", " client->scene 地牢开宝箱请求")]
	public class LostDungeonOpenBoxReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 510016;
		public UInt32 Sequence;

		public UInt32 floor;

		public UInt32 boxId;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, floor);
			BaseDLL.encode_uint32(buffer, ref pos_, boxId);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref floor);
			BaseDLL.decode_uint32(buffer, ref pos_, ref boxId);
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
