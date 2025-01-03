using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 设置关卡药水配置
	/// </summary>
	[AdvancedInspector.Descriptor("设置关卡药水配置", "设置关卡药水配置")]
	public class SceneSetDungeonPotionReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 500964;
		public UInt32 Sequence;

		public UInt32 potionId;

		public byte pos;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, potionId);
			BaseDLL.encode_int8(buffer, ref pos_, pos);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref potionId);
			BaseDLL.decode_int8(buffer, ref pos_, ref pos);
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
