using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  地下城静态数值
	/// </summary>
	[AdvancedInspector.Descriptor(" 地下城静态数值", " 地下城静态数值")]
	public class DungeonStaticValue : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public Int32[] values = new Int32[0];

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)values.Length);
			for(int i = 0; i < values.Length; i++)
			{
				BaseDLL.encode_int32(buffer, ref pos_, values[i]);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 valuesCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref valuesCnt);
			values = new Int32[valuesCnt];
			for(int i = 0; i < values.Length; i++)
			{
				BaseDLL.decode_int32(buffer, ref pos_, ref values[i]);
			}
		}


		#endregion

	}

}
