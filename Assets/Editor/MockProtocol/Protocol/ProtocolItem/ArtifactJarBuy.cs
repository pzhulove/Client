using System;
using System.Text;

namespace Mock.Protocol
{

	public class ArtifactJarBuy : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  罐子id
		/// </summary>
		[AdvancedInspector.Descriptor(" 罐子id", " 罐子id")]
		public UInt32 jarId;
		/// <summary>
		///  购买次数
		/// </summary>
		[AdvancedInspector.Descriptor(" 购买次数", " 购买次数")]
		public UInt32 buyCnt;
		/// <summary>
		///  总次数
		/// </summary>
		[AdvancedInspector.Descriptor(" 总次数", " 总次数")]
		public UInt32 totalCnt;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, jarId);
			BaseDLL.encode_uint32(buffer, ref pos_, buyCnt);
			BaseDLL.encode_uint32(buffer, ref pos_, totalCnt);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref jarId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref buyCnt);
			BaseDLL.decode_uint32(buffer, ref pos_, ref totalCnt);
		}


		#endregion

	}

}
