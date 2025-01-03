using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  账号商店查询索引
	/// </summary>
	[AdvancedInspector.Descriptor(" 账号商店查询索引", " 账号商店查询索引")]
	public class AccountShopQueryIndex : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  查询的商店id
		/// </summary>
		[AdvancedInspector.Descriptor(" 查询的商店id", " 查询的商店id")]
		public UInt32 shopId;
		/// <summary>
		///  查询的页签类别
		/// </summary>
		[AdvancedInspector.Descriptor(" 查询的页签类别", " 查询的页签类别")]
		public byte tabType;
		/// <summary>
		///  查询的职业类别
		/// </summary>
		[AdvancedInspector.Descriptor(" 查询的职业类别", " 查询的职业类别")]
		public byte jobType;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, shopId);
			BaseDLL.encode_int8(buffer, ref pos_, tabType);
			BaseDLL.encode_int8(buffer, ref pos_, jobType);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref shopId);
			BaseDLL.decode_int8(buffer, ref pos_, ref tabType);
			BaseDLL.decode_int8(buffer, ref pos_, ref jobType);
		}


		#endregion

	}

}
