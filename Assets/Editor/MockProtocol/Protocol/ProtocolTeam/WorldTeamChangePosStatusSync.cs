using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  同步位置状态改变
	/// </summary>
	[AdvancedInspector.Descriptor(" 同步位置状态改变", " 同步位置状态改变")]
	public class WorldTeamChangePosStatusSync : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601631;
		public UInt32 Sequence;
		/// <summary>
		///  位置
		/// </summary>
		[AdvancedInspector.Descriptor(" 位置", " 位置")]
		public byte pos;
		/// <summary>
		///  1代表打开位置，0代表关闭位置
		/// </summary>
		[AdvancedInspector.Descriptor(" 1代表打开位置，0代表关闭位置", " 1代表打开位置，0代表关闭位置")]
		public byte open;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, pos);
			BaseDLL.encode_int8(buffer, ref pos_, open);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref pos);
			BaseDLL.decode_int8(buffer, ref pos_, ref open);
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
