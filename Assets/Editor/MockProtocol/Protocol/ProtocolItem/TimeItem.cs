using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  到期被删除
	/// </summary>
	[AdvancedInspector.Descriptor(" 到期被删除", " 到期被删除")]
	public class TimeItem : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt64 itemUid;

		public UInt32 state;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, itemUid);
			BaseDLL.encode_uint32(buffer, ref pos_, state);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref itemUid);
			BaseDLL.decode_uint32(buffer, ref pos_, ref state);
		}


		#endregion

	}

}
