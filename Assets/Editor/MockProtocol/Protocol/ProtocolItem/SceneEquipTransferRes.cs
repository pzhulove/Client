using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// server->client 装备传家返回
	/// </summary>
	[AdvancedInspector.Descriptor("server->client 装备传家返回", "server->client 装备传家返回")]
	public class SceneEquipTransferRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501018;
		public UInt32 Sequence;
		/// <summary>
		/// 错误码
		/// </summary>
		[AdvancedInspector.Descriptor("错误码", "错误码")]
		public UInt32 code;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, code);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref code);
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
