using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  返回打开红包请求
	/// </summary>
	[AdvancedInspector.Descriptor(" 返回打开红包请求", " 返回打开红包请求")]
	public class WorldOpenRedPacketRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 607309;
		public UInt32 Sequence;
		/// <summary>
		///  返回码
		/// </summary>
		[AdvancedInspector.Descriptor(" 返回码", " 返回码")]
		public UInt32 result;
		/// <summary>
		///  红包详细信息
		/// </summary>
		[AdvancedInspector.Descriptor(" 红包详细信息", " 红包详细信息")]
		public RedPacketDetail detail = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, result);
			detail.encode(buffer, ref pos_);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			detail.decode(buffer, ref pos_);
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
