using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 商城道具id
	/// </summary>
	[AdvancedInspector.Descriptor("商城道具id", "商城道具id")]
	public class MallGiftDetail : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt32 itemId;

		public UInt16 num;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, itemId);
			BaseDLL.encode_uint16(buffer, ref pos_, num);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref itemId);
			BaseDLL.decode_uint16(buffer, ref pos_, ref num);
		}


		#endregion

	}

}
