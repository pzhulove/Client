using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 随机属性: 属性值，buff:无用
	/// </summary>
	[AdvancedInspector.Descriptor("随机属性: 属性值，buff:无用", "随机属性: 属性值，buff:无用")]
	public class ItemMountedMagic : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt32 magicCardId;
		/// <summary>
		/// 附魔卡id
		/// </summary>
		[AdvancedInspector.Descriptor("附魔卡id", "附魔卡id")]
		public byte level;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, magicCardId);
			BaseDLL.encode_int8(buffer, ref pos_, level);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref magicCardId);
			BaseDLL.decode_int8(buffer, ref pos_, ref level);
		}


		#endregion

	}

}
