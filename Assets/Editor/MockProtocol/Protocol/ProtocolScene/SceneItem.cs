using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  场景物品
	/// </summary>
	[AdvancedInspector.Descriptor(" 场景物品", " 场景物品")]
	public class SceneItem : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  唯一ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 唯一ID", " 唯一ID")]
		public UInt64 guid;
		/// <summary>
		///  类型ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 类型ID", " 类型ID")]
		public UInt32 data_id;
		/// <summary>
		///  位置
		/// </summary>
		[AdvancedInspector.Descriptor(" 位置", " 位置")]
		public SceneItemPos pos = null;
		/// <summary>
		///  归属
		/// </summary>
		[AdvancedInspector.Descriptor(" 归属", " 归属")]
		public UInt64 owner;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, guid);
			BaseDLL.encode_uint32(buffer, ref pos_, data_id);
			pos.encode(buffer, ref pos_);
			BaseDLL.encode_uint64(buffer, ref pos_, owner);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref guid);
			BaseDLL.decode_uint32(buffer, ref pos_, ref data_id);
			pos.decode(buffer, ref pos_);
			BaseDLL.decode_uint64(buffer, ref pos_, ref owner);
		}


		#endregion

	}

}
