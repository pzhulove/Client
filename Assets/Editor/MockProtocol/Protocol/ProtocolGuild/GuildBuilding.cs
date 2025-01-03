using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  公会建筑
	/// </summary>
	[AdvancedInspector.Descriptor(" 公会建筑", " 公会建筑")]
	public class GuildBuilding : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  建筑类型（对应枚举GuildBuildingType）
		/// </summary>
		[AdvancedInspector.Descriptor(" 建筑类型（对应枚举GuildBuildingType）", " 建筑类型（对应枚举GuildBuildingType）")]
		public byte type;
		/// <summary>
		///  等级
		/// </summary>
		[AdvancedInspector.Descriptor(" 等级", " 等级")]
		public byte level;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, type);
			BaseDLL.encode_int8(buffer, ref pos_, level);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref type);
			BaseDLL.decode_int8(buffer, ref pos_, ref level);
		}


		#endregion

	}

}
