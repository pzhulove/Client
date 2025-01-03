using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  client->scene 同步抗魔值
	/// </summary>
	[AdvancedInspector.Descriptor(" client->scene 同步抗魔值", " client->scene 同步抗魔值")]
	public class SceneSyncResistMagicReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501021;
		public UInt32 Sequence;

		public UInt32 resist_magic;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, resist_magic);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref resist_magic);
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
