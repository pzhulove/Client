using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 镶嵌宝珠信息
	/// </summary>
	[AdvancedInspector.Descriptor("镶嵌宝珠信息", "镶嵌宝珠信息")]
	public class RacePrecBead : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt32 preciousBeadId;

		public UInt32 buffId;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, preciousBeadId);
			BaseDLL.encode_uint32(buffer, ref pos_, buffId);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref preciousBeadId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref buffId);
		}


		#endregion

	}

}
