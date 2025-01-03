using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// client->scene 装备宝珠摘除请求
	/// </summary>
	[AdvancedInspector.Descriptor("client->scene 装备宝珠摘除请求", "client->scene 装备宝珠摘除请求")]
	public class SceneExtirpePreciousBeadReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501035;
		public UInt32 Sequence;
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
		/// <summary>
		///  杵id
		/// </summary>
		[AdvancedInspector.Descriptor(" 杵id", " 杵id")]
		public UInt32 pestleId;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, itemUid);
			BaseDLL.encode_int8(buffer, ref pos_, holeIndex);
			BaseDLL.encode_uint32(buffer, ref pos_, pestleId);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref itemUid);
			BaseDLL.decode_int8(buffer, ref pos_, ref holeIndex);
			BaseDLL.decode_uint32(buffer, ref pos_, ref pestleId);
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
