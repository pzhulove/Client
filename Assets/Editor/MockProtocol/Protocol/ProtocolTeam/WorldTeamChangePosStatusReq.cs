using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  请求修改位置状态（打开或关闭）
	/// </summary>
	[AdvancedInspector.Descriptor(" 请求修改位置状态（打开或关闭）", " 请求修改位置状态（打开或关闭）")]
	public class WorldTeamChangePosStatusReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601630;
		public UInt32 Sequence;
		/// <summary>
		///  位置
		/// </summary>
		[AdvancedInspector.Descriptor(" 位置", " 位置")]
		public byte pos;
		/// <summary>
		///  0代表打开位置，1代表关闭位置
		/// </summary>
		[AdvancedInspector.Descriptor(" 0代表打开位置，1代表关闭位置", " 0代表打开位置，1代表关闭位置")]
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
