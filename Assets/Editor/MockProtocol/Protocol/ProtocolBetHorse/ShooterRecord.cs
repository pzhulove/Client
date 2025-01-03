using System;
using System.Text;

namespace Mock.Protocol
{

	public class ShooterRecord : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  场次
		/// </summary>
		[AdvancedInspector.Descriptor(" 场次", " 场次")]
		public UInt32 coutrId;
		/// <summary>
		///  自己的赔率
		/// </summary>
		[AdvancedInspector.Descriptor(" 自己的赔率", " 自己的赔率")]
		public UInt32 odds;
		/// <summary>
		///  胜负结果
		/// </summary>
		[AdvancedInspector.Descriptor(" 胜负结果", " 胜负结果")]
		public UInt32 result;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, coutrId);
			BaseDLL.encode_uint32(buffer, ref pos_, odds);
			BaseDLL.encode_uint32(buffer, ref pos_, result);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref coutrId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref odds);
			BaseDLL.decode_uint32(buffer, ref pos_, ref result);
		}


		#endregion

	}

}
