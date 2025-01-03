using System;
using System.Text;

namespace Mock.Protocol
{

	public class HeadFrame : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  头像框id
		/// </summary>
		[AdvancedInspector.Descriptor(" 头像框id", " 头像框id")]
		public UInt32 headFrameId;
		/// <summary>
		///  过期时间
		/// </summary>
		[AdvancedInspector.Descriptor(" 过期时间", " 过期时间")]
		public UInt32 expireTime;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, headFrameId);
			BaseDLL.encode_uint32(buffer, ref pos_, expireTime);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref headFrameId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref expireTime);
		}


		#endregion

	}

}
