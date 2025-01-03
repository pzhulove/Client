using System;
using System.Text;

namespace Mock.Protocol
{

	public class OddsRateInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  射手id
		/// </summary>
		[AdvancedInspector.Descriptor(" 射手id", " 射手id")]
		public UInt32 id;
		/// <summary>
		///  赔率
		/// </summary>
		[AdvancedInspector.Descriptor(" 赔率", " 赔率")]
		public UInt32 odds;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, id);
			BaseDLL.encode_uint32(buffer, ref pos_, odds);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref id);
			BaseDLL.decode_uint32(buffer, ref pos_, ref odds);
		}


		#endregion

	}

}
