using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  npc坐标
	/// </summary>
	[AdvancedInspector.Descriptor(" npc坐标", " npc坐标")]
	public class NpcPos : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public Int32 x;

		public Int32 y;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int32(buffer, ref pos_, x);
			BaseDLL.encode_int32(buffer, ref pos_, y);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int32(buffer, ref pos_, ref x);
			BaseDLL.decode_int32(buffer, ref pos_, ref y);
		}


		#endregion

	}

}
