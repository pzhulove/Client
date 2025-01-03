using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// client->world 领取查询经验包
	/// </summary>
	[AdvancedInspector.Descriptor("client->world 领取查询经验包", "client->world 领取查询经验包")]
	public class WorldAventurePassExpPackReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 609507;
		public UInt32 Sequence;
		/// <summary>
		/// 0 是查询 1是领取
		/// </summary>
		[AdvancedInspector.Descriptor("0 是查询 1是领取", "0 是查询 1是领取")]
		public byte op;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, op);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref op);
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
