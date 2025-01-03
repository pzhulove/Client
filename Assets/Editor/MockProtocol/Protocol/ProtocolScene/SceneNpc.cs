using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  城镇npc（目前就支持城镇怪物）
	/// </summary>
	[AdvancedInspector.Descriptor(" 城镇npc（目前就支持城镇怪物）", " 城镇npc（目前就支持城镇怪物）")]
	public class SceneNpc : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  唯一ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 唯一ID", " 唯一ID")]
		public UInt64 guid;
		/// <summary>
		///  npc类型，对应枚举SceneObjectType
		/// </summary>
		[AdvancedInspector.Descriptor(" npc类型，对应枚举SceneObjectType", " npc类型，对应枚举SceneObjectType")]
		public byte type;
		/// <summary>
		///  功能类型（不同SceneObjectType有不同的含义）
		/// </summary>
		/// <summary>
		///  SOT_CITYMONSTER -> CityMonsterType
		/// </summary>
		[AdvancedInspector.Descriptor(" SOT_CITYMONSTER -> CityMonsterType", " SOT_CITYMONSTER -> CityMonsterType")]
		public byte funcType;
		/// <summary>
		///  类型ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 类型ID", " 类型ID")]
		public UInt32 id;
		/// <summary>
		///  位置
		/// </summary>
		[AdvancedInspector.Descriptor(" 位置", " 位置")]
		public NpcPos pos = null;
		/// <summary>
		///  剩余次数
		/// </summary>
		[AdvancedInspector.Descriptor(" 剩余次数", " 剩余次数")]
		public UInt32 remainTimes;
		/// <summary>
		///  总次数
		/// </summary>
		[AdvancedInspector.Descriptor(" 总次数", " 总次数")]
		public UInt32 totalTimes;
		/// <summary>
		///  对应的地下城
		/// </summary>
		[AdvancedInspector.Descriptor(" 对应的地下城", " 对应的地下城")]
		public UInt32 dungeonId;
		/// <summary>
		///  是否战斗中
		/// </summary>
		[AdvancedInspector.Descriptor(" 是否战斗中", " 是否战斗中")]
		public byte inBattle;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, guid);
			BaseDLL.encode_int8(buffer, ref pos_, type);
			BaseDLL.encode_int8(buffer, ref pos_, funcType);
			BaseDLL.encode_uint32(buffer, ref pos_, id);
			pos.encode(buffer, ref pos_);
			BaseDLL.encode_uint32(buffer, ref pos_, remainTimes);
			BaseDLL.encode_uint32(buffer, ref pos_, totalTimes);
			BaseDLL.encode_uint32(buffer, ref pos_, dungeonId);
			BaseDLL.encode_int8(buffer, ref pos_, inBattle);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref guid);
			BaseDLL.decode_int8(buffer, ref pos_, ref type);
			BaseDLL.decode_int8(buffer, ref pos_, ref funcType);
			BaseDLL.decode_uint32(buffer, ref pos_, ref id);
			pos.decode(buffer, ref pos_);
			BaseDLL.decode_uint32(buffer, ref pos_, ref remainTimes);
			BaseDLL.decode_uint32(buffer, ref pos_, ref totalTimes);
			BaseDLL.decode_uint32(buffer, ref pos_, ref dungeonId);
			BaseDLL.decode_int8(buffer, ref pos_, ref inBattle);
		}


		#endregion

	}

}
