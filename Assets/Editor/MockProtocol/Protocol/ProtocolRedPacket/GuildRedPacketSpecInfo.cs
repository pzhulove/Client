using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  公会红包信息
	/// </summary>
	[AdvancedInspector.Descriptor(" 公会红包信息", " 公会红包信息")]
	public class GuildRedPacketSpecInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  红包类型
		/// </summary>
		[AdvancedInspector.Descriptor(" 红包类型", " 红包类型")]
		public byte type;
		/// <summary>
		///  时间
		/// </summary>
		[AdvancedInspector.Descriptor(" 时间", " 时间")]
		public UInt32 lastTime;
		/// <summary>
		///  参与人数
		/// </summary>
		[AdvancedInspector.Descriptor(" 参与人数", " 参与人数")]
		public UInt32 joinNum;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, type);
			BaseDLL.encode_uint32(buffer, ref pos_, lastTime);
			BaseDLL.encode_uint32(buffer, ref pos_, joinNum);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref type);
			BaseDLL.decode_uint32(buffer, ref pos_, ref lastTime);
			BaseDLL.decode_uint32(buffer, ref pos_, ref joinNum);
		}


		#endregion

	}

}
