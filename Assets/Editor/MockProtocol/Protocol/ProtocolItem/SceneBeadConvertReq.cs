using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  宝珠转换请求
	/// </summary>
	[AdvancedInspector.Descriptor(" 宝珠转换请求", " 宝珠转换请求")]
	public class SceneBeadConvertReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501090;
		public UInt32 Sequence;
		/// <summary>
		///  转换的宝珠
		/// </summary>
		[AdvancedInspector.Descriptor(" 转换的宝珠", " 转换的宝珠")]
		public UInt64 beadGuid;
		/// <summary>
		///  使用的材料(0使用金币)
		/// </summary>
		[AdvancedInspector.Descriptor(" 使用的材料(0使用金币)", " 使用的材料(0使用金币)")]
		public UInt64 materialGuid;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, beadGuid);
			BaseDLL.encode_uint64(buffer, ref pos_, materialGuid);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref beadGuid);
			BaseDLL.decode_uint64(buffer, ref pos_, ref materialGuid);
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
