using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 押注记录
	/// </summary>
	[AdvancedInspector.Descriptor("押注记录", "押注记录")]
	public class GambleRecord : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		/// 押的项目
		/// </summary>
		[AdvancedInspector.Descriptor("押的项目", "押的项目")]
		public UInt32 id;
		/// <summary>
		/// 押的选项
		/// </summary>
		[AdvancedInspector.Descriptor("押的选项", "押的选项")]
		public UInt64 option;
		/// <summary>
		/// 胜利的选项
		/// </summary>
		[AdvancedInspector.Descriptor("胜利的选项", "胜利的选项")]
		public UInt64 result;
		/// <summary>
		/// 获得竞猜币数量
		/// </summary>
		[AdvancedInspector.Descriptor("获得竞猜币数量", "获得竞猜币数量")]
		public UInt64 reward;
		/// <summary>
		/// 下注时间
		/// </summary>
		[AdvancedInspector.Descriptor("下注时间", "下注时间")]
		public UInt32 time;
		/// <summary>
		/// 押注数量
		/// </summary>
		[AdvancedInspector.Descriptor("押注数量", "押注数量")]
		public UInt64 num;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, id);
			BaseDLL.encode_uint64(buffer, ref pos_, option);
			BaseDLL.encode_uint64(buffer, ref pos_, result);
			BaseDLL.encode_uint64(buffer, ref pos_, reward);
			BaseDLL.encode_uint32(buffer, ref pos_, time);
			BaseDLL.encode_uint64(buffer, ref pos_, num);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref id);
			BaseDLL.decode_uint64(buffer, ref pos_, ref option);
			BaseDLL.decode_uint64(buffer, ref pos_, ref result);
			BaseDLL.decode_uint64(buffer, ref pos_, ref reward);
			BaseDLL.decode_uint32(buffer, ref pos_, ref time);
			BaseDLL.decode_uint64(buffer, ref pos_, ref num);
		}


		#endregion

	}

}
