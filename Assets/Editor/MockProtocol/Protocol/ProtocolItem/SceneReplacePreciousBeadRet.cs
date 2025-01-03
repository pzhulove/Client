using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  宝珠替换返回
	/// </summary>
	[AdvancedInspector.Descriptor(" 宝珠替换返回", " 宝珠替换返回")]
	public class SceneReplacePreciousBeadRet : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501043;
		public UInt32 Sequence;
		/// <summary>
		/// 返回码
		/// </summary>
		[AdvancedInspector.Descriptor("返回码", "返回码")]
		public UInt32 retCode;
		/// <summary>
		///  宝珠id
		/// </summary>
		[AdvancedInspector.Descriptor(" 宝珠id", " 宝珠id")]
		public UInt32 preciousBeadId;
		/// <summary>
		///  道具uid
		/// </summary>
		[AdvancedInspector.Descriptor(" 道具uid", " 道具uid")]
		public UInt64 itemUid;
		/// <summary>
		///  孔索引
		/// </summary>
		[AdvancedInspector.Descriptor(" 孔索引", " 孔索引")]
		public byte holeIndex;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, retCode);
			BaseDLL.encode_uint32(buffer, ref pos_, preciousBeadId);
			BaseDLL.encode_uint64(buffer, ref pos_, itemUid);
			BaseDLL.encode_int8(buffer, ref pos_, holeIndex);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
			BaseDLL.decode_uint32(buffer, ref pos_, ref preciousBeadId);
			BaseDLL.decode_uint64(buffer, ref pos_, ref itemUid);
			BaseDLL.decode_int8(buffer, ref pos_, ref holeIndex);
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
