using System;
using System.Text;

namespace Mock.Protocol
{

	public class FrameChecksum : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  帧序号
		/// </summary>
		[AdvancedInspector.Descriptor(" 帧序号", " 帧序号")]
		public UInt32 frame;
		/// <summary>
		///  帧校验值
		/// </summary>
		[AdvancedInspector.Descriptor(" 帧校验值", " 帧校验值")]
		public UInt32 checksum;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, frame);
			BaseDLL.encode_uint32(buffer, ref pos_, checksum);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref frame);
			BaseDLL.decode_uint32(buffer, ref pos_, ref checksum);
		}


		#endregion

	}

}
