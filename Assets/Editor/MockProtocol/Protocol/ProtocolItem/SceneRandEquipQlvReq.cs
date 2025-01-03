using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 需要材料数量
	/// </summary>
	[AdvancedInspector.Descriptor("需要材料数量", "需要材料数量")]
	public class SceneRandEquipQlvReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 500941;
		public UInt32 Sequence;

		public UInt64 uid;
		/// <summary>
		/// 装备uid
		/// </summary>
		[AdvancedInspector.Descriptor("装备uid", "装备uid")]
		public byte bUsePoint;
		/// <summary>
		/// 是否使用绑点代替
		/// </summary>
		[AdvancedInspector.Descriptor("是否使用绑点代替", "是否使用绑点代替")]
		public byte usePerfect;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, uid);
			BaseDLL.encode_int8(buffer, ref pos_, bUsePoint);
			BaseDLL.encode_int8(buffer, ref pos_, usePerfect);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref uid);
			BaseDLL.decode_int8(buffer, ref pos_, ref bUsePoint);
			BaseDLL.decode_int8(buffer, ref pos_, ref usePerfect);
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
