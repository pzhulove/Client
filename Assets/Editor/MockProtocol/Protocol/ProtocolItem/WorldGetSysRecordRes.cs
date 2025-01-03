using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  拉取系统记录返回
	/// </summary>
	[AdvancedInspector.Descriptor(" 拉取系统记录返回", " 拉取系统记录返回")]
	public class WorldGetSysRecordRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 600908;
		public UInt32 Sequence;

		public UInt32 behavoir;

		public UInt32 role;

		public UInt32 param;

		public UInt32 value;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, behavoir);
			BaseDLL.encode_uint32(buffer, ref pos_, role);
			BaseDLL.encode_uint32(buffer, ref pos_, param);
			BaseDLL.encode_uint32(buffer, ref pos_, value);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref behavoir);
			BaseDLL.decode_uint32(buffer, ref pos_, ref role);
			BaseDLL.decode_uint32(buffer, ref pos_, ref param);
			BaseDLL.decode_uint32(buffer, ref pos_, ref value);
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
