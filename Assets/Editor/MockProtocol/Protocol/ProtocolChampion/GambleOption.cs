using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 押注选项
	/// </summary>
	[AdvancedInspector.Descriptor("押注选项", "押注选项")]
	public class GambleOption : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		/// 本选项
		/// </summary>
		[AdvancedInspector.Descriptor("本选项", "本选项")]
		public UInt64 option;
		/// <summary>
		/// 竞猜币数量
		/// </summary>
		[AdvancedInspector.Descriptor("竞猜币数量", "竞猜币数量")]
		public UInt64 num;
		/// <summary>
		/// 赔率
		/// </summary>
		[AdvancedInspector.Descriptor("赔率", "赔率")]
		public UInt32 odds;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, option);
			BaseDLL.encode_uint64(buffer, ref pos_, num);
			BaseDLL.encode_uint32(buffer, ref pos_, odds);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref option);
			BaseDLL.decode_uint64(buffer, ref pos_, ref num);
			BaseDLL.decode_uint32(buffer, ref pos_, ref odds);
		}


		#endregion

	}

}
