using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// cliet->server 装备传家请求
	/// </summary>
	[AdvancedInspector.Descriptor("cliet->server 装备传家请求", "cliet->server 装备传家请求")]
	public class SceneEquipTransferReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501017;
		public UInt32 Sequence;
		/// <summary>
		/// 装备uid
		/// </summary>
		[AdvancedInspector.Descriptor("装备uid", "装备uid")]
		public UInt64 equid;
		/// <summary>
		/// 转移石uid
		/// </summary>
		[AdvancedInspector.Descriptor("转移石uid", "转移石uid")]
		public UInt64 transferId;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, equid);
			BaseDLL.encode_uint64(buffer, ref pos_, transferId);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref equid);
			BaseDLL.decode_uint64(buffer, ref pos_, ref transferId);
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
