using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  拉取系统记录请求
	/// </summary>
	[AdvancedInspector.Descriptor(" 拉取系统记录请求", " 拉取系统记录请求")]
	public class WorldGetSysRecordReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 600907;
		public UInt32 Sequence;

		public UInt32 behavoir;

		public UInt32 role;

		public UInt32 param;

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
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref behavoir);
			BaseDLL.decode_uint32(buffer, ref pos_, ref role);
			BaseDLL.decode_uint32(buffer, ref pos_, ref param);
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
