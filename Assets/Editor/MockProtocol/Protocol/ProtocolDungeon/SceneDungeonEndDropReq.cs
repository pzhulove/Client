using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  请求结算掉落
	/// </summary>
	[AdvancedInspector.Descriptor(" 请求结算掉落", " 请求结算掉落")]
	public class SceneDungeonEndDropReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 506823;
		public UInt32 Sequence;
		/// <summary>
		///  倍率
		/// </summary>
		[AdvancedInspector.Descriptor(" 倍率", " 倍率")]
		public byte multi;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, multi);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref multi);
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
