using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  宝珠替换请求
	/// </summary>
	[AdvancedInspector.Descriptor(" 宝珠替换请求", " 宝珠替换请求")]
	public class SceneReplacePreciousBeadReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501042;
		public UInt32 Sequence;
		/// <summary>
		///  道具uid
		/// </summary>
		[AdvancedInspector.Descriptor(" 道具uid", " 道具uid")]
		public UInt64 itemUid;
		/// <summary>
		///  宝珠uid
		/// </summary>
		[AdvancedInspector.Descriptor(" 宝珠uid", " 宝珠uid")]
		public UInt64 preciousBeadUid;
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
			BaseDLL.encode_uint64(buffer, ref pos_, itemUid);
			BaseDLL.encode_uint64(buffer, ref pos_, preciousBeadUid);
			BaseDLL.encode_int8(buffer, ref pos_, holeIndex);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref itemUid);
			BaseDLL.decode_uint64(buffer, ref pos_, ref preciousBeadUid);
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
