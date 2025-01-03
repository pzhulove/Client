using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 装备升级返回
	/// </summary>
	[AdvancedInspector.Descriptor("装备升级返回", "装备升级返回")]
	public class SceneEquieUpdateRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501049;
		public UInt32 Sequence;

		public UInt32 code;

		public UInt64 equipUid;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, code);
			BaseDLL.encode_uint64(buffer, ref pos_, equipUid);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			BaseDLL.decode_uint64(buffer, ref pos_, ref equipUid);
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
