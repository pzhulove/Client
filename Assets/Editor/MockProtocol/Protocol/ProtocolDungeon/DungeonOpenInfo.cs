using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  地下城开放信息
	/// </summary>
	[AdvancedInspector.Descriptor(" 地下城开放信息", " 地下城开放信息")]
	public class DungeonOpenInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  地下城ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 地下城ID", " 地下城ID")]
		public UInt32 id;
		/// <summary>
		///  是否开放深渊模式(1:开放，0:不开放)
		/// </summary>
		[AdvancedInspector.Descriptor(" 是否开放深渊模式(1:开放，0:不开放)", " 是否开放深渊模式(1:开放，0:不开放)")]
		public byte hellMode;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, id);
			BaseDLL.encode_int8(buffer, ref pos_, hellMode);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref id);
			BaseDLL.decode_int8(buffer, ref pos_, ref hellMode);
		}


		#endregion

	}

}
