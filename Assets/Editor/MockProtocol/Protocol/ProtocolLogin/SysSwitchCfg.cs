using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  开关结构
	/// </summary>
	[AdvancedInspector.Descriptor(" 开关结构", " 开关结构")]
	public class SysSwitchCfg : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt32 sysType;

		public byte switchValue;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, sysType);
			BaseDLL.encode_int8(buffer, ref pos_, switchValue);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref sysType);
			BaseDLL.decode_int8(buffer, ref pos_, ref switchValue);
		}


		#endregion

	}

}
