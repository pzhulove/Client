using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  物品基本信息
	/// </summary>
	[AdvancedInspector.Descriptor(" 物品基本信息", " 物品基本信息")]
	public class ItemBaseInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  唯一ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 唯一ID", " 唯一ID")]
		public UInt64 id;
		/// <summary>
		///  类型ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 类型ID", " 类型ID")]
		public UInt32 typeId;
		/// <summary>
		///  位置
		/// </summary>
		[AdvancedInspector.Descriptor(" 位置", " 位置")]
		public UInt32 pos;
		/// <summary>
		///  强化
		/// </summary>
		[AdvancedInspector.Descriptor(" 强化", " 强化")]
		public byte strengthen;
		/// <summary>
		///  装备类型
		/// </summary>
		[AdvancedInspector.Descriptor(" 装备类型", " 装备类型")]
		public byte equipType;
		/// <summary>
		/// 增幅路线
		/// </summary>
		[AdvancedInspector.Descriptor("增幅路线", "增幅路线")]
		public byte enhanceType;
		/// <summary>
		///  装备评分
		/// </summary>
		[AdvancedInspector.Descriptor(" 装备评分", " 装备评分")]
		public UInt32 equipScore;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, id);
			BaseDLL.encode_uint32(buffer, ref pos_, typeId);
			BaseDLL.encode_uint32(buffer, ref pos_, pos);
			BaseDLL.encode_int8(buffer, ref pos_, strengthen);
			BaseDLL.encode_int8(buffer, ref pos_, equipType);
			BaseDLL.encode_int8(buffer, ref pos_, enhanceType);
			BaseDLL.encode_uint32(buffer, ref pos_, equipScore);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref id);
			BaseDLL.decode_uint32(buffer, ref pos_, ref typeId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref pos);
			BaseDLL.decode_int8(buffer, ref pos_, ref strengthen);
			BaseDLL.decode_int8(buffer, ref pos_, ref equipType);
			BaseDLL.decode_int8(buffer, ref pos_, ref enhanceType);
			BaseDLL.decode_uint32(buffer, ref pos_, ref equipScore);
		}


		#endregion

	}

}
