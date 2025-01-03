using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 地牢玩家坐标信息
	/// </summary>
	[AdvancedInspector.Descriptor("地牢玩家坐标信息", "地牢玩家坐标信息")]
	public class LostDungeonPlayerPos : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		/// 玩家id
		/// </summary>
		[AdvancedInspector.Descriptor("玩家id", "玩家id")]
		public UInt64 playerId;
		/// <summary>
		/// 场景id
		/// </summary>
		[AdvancedInspector.Descriptor("场景id", "场景id")]
		public UInt32 sceneId;
		/// <summary>
		/// x坐标
		/// </summary>
		[AdvancedInspector.Descriptor("x坐标", "x坐标")]
		public UInt32 posX;
		/// <summary>
		/// y坐标
		/// </summary>
		[AdvancedInspector.Descriptor("y坐标", "y坐标")]
		public UInt32 posY;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, playerId);
			BaseDLL.encode_uint32(buffer, ref pos_, sceneId);
			BaseDLL.encode_uint32(buffer, ref pos_, posX);
			BaseDLL.encode_uint32(buffer, ref pos_, posY);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref playerId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref sceneId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref posX);
			BaseDLL.decode_uint32(buffer, ref pos_, ref posY);
		}


		#endregion

	}

}
