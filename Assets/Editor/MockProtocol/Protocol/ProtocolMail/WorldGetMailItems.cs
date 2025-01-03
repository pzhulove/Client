using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  client->server 收取邮件附件
	/// </summary>
	[AdvancedInspector.Descriptor(" client->server 收取邮件附件", " client->server 收取邮件附件")]
	public class WorldGetMailItems : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601506;
		public UInt32 Sequence;
		/// <summary>
		/// 类型 0为收取单个 1为全部收取
		/// </summary>
		[AdvancedInspector.Descriptor("类型 0为收取单个 1为全部收取", "类型 0为收取单个 1为全部收取")]
		public byte type;

		public UInt64 id;

		public UInt32 mailType;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, type);
			BaseDLL.encode_uint64(buffer, ref pos_, id);
			BaseDLL.encode_uint32(buffer, ref pos_, mailType);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref type);
			BaseDLL.decode_uint64(buffer, ref pos_, ref id);
			BaseDLL.decode_uint32(buffer, ref pos_, ref mailType);
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
