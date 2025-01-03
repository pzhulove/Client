using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 合成的附魔卡等级
	/// </summary>
	/// <summary>
	/// 添加宝珠请求
	/// </summary>
	[AdvancedInspector.Descriptor("添加宝珠请求", "添加宝珠请求")]
	public class SceneAddPreciousBeadReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 500971;
		public UInt32 Sequence;
		/// <summary>
		///  宝珠uid
		/// </summary>
		[AdvancedInspector.Descriptor(" 宝珠uid", " 宝珠uid")]
		public UInt64 preciousBeadUid;
		/// <summary>
		///  道具uid
		/// </summary>
		[AdvancedInspector.Descriptor(" 道具uid", " 道具uid")]
		public UInt64 itemUid;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, preciousBeadUid);
			BaseDLL.encode_uint64(buffer, ref pos_, itemUid);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref preciousBeadUid);
			BaseDLL.decode_uint64(buffer, ref pos_, ref itemUid);
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
