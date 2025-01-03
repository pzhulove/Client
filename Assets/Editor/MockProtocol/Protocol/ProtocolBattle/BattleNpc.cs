using System;
using System.Text;

namespace Mock.Protocol
{

	public class BattleNpc : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt64 npcGuid;
		/// <summary>
		///  npcId
		/// </summary>
		[AdvancedInspector.Descriptor(" npcId", " npcId")]
		public UInt32 npcId;
		/// <summary>
		///  1刷出，0清除
		/// </summary>
		[AdvancedInspector.Descriptor(" 1刷出，0清除", " 1刷出，0清除")]
		public UInt32 opType;
		/// <summary>
		///  位置
		/// </summary>
		[AdvancedInspector.Descriptor(" 位置", " 位置")]
		public ScenePosition pos = null;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, npcGuid);
			BaseDLL.encode_uint32(buffer, ref pos_, npcId);
			BaseDLL.encode_uint32(buffer, ref pos_, opType);
			pos.encode(buffer, ref pos_);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref npcGuid);
			BaseDLL.decode_uint32(buffer, ref pos_, ref npcId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref opType);
			pos.decode(buffer, ref pos_);
		}


		#endregion

	}

}
