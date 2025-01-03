using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  据点状态通知
	/// </summary>
	[AdvancedInspector.Descriptor(" 据点状态通知", " 据点状态通知")]
	public class TeamCopyFieldStatusNotify : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 1100064;
		public UInt32 Sequence;
		/// <summary>
		///  据点id
		/// </summary>
		[AdvancedInspector.Descriptor(" 据点id", " 据点id")]
		public UInt32 fieldId;
		/// <summary>
		///  据点状态
		/// </summary>
		[AdvancedInspector.Descriptor(" 据点状态", " 据点状态")]
		public UInt32 fieldStatus;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, fieldId);
			BaseDLL.encode_uint32(buffer, ref pos_, fieldStatus);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref fieldId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref fieldStatus);
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
