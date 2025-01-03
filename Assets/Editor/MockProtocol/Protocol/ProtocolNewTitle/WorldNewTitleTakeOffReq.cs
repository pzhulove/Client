using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 唯一id
	/// </summary>
	/// <summary>
	/// client->world 头衔脱掉请求
	/// </summary>
	[AdvancedInspector.Descriptor("client->world 头衔脱掉请求", "client->world 头衔脱掉请求")]
	public class WorldNewTitleTakeOffReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 609204;
		public UInt32 Sequence;

		public UInt64 titleGuid;
		/// <summary>
		/// 唯一id
		/// </summary>
		[AdvancedInspector.Descriptor("唯一id", "唯一id")]
		public UInt32 titleId;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, titleGuid);
			BaseDLL.encode_uint32(buffer, ref pos_, titleId);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref titleGuid);
			BaseDLL.decode_uint32(buffer, ref pos_, ref titleId);
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
