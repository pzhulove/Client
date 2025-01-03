using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  同步队长操作
	/// </summary>
	[AdvancedInspector.Descriptor(" 同步队长操作", " 同步队长操作")]
	public class WorldTeamMasterOperSync : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601629;
		public UInt32 Sequence;
		/// <summary>
		///  操作类型
		/// </summary>
		[AdvancedInspector.Descriptor(" 操作类型", " 操作类型")]
		public byte type;
		/// <summary>
		///  具体操作
		/// </summary>
		[AdvancedInspector.Descriptor(" 具体操作", " 具体操作")]
		public UInt32 param;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, type);
			BaseDLL.encode_uint32(buffer, ref pos_, param);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref type);
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
