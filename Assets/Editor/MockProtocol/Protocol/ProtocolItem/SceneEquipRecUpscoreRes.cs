using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// server->client 装备回收积分提升返回
	/// </summary>
	[AdvancedInspector.Descriptor("server->client 装备回收积分提升返回", "server->client 装备回收积分提升返回")]
	public class SceneEquipRecUpscoreRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501013;
		public UInt32 Sequence;
		/// <summary>
		/// 错误码
		/// </summary>
		[AdvancedInspector.Descriptor("错误码", "错误码")]
		public UInt32 code;
		/// <summary>
		/// 提升积分值
		/// </summary>
		[AdvancedInspector.Descriptor("提升积分值", "提升积分值")]
		public UInt32 upscore;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, code);
			BaseDLL.encode_uint32(buffer, ref pos_, upscore);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			BaseDLL.decode_uint32(buffer, ref pos_, ref upscore);
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
