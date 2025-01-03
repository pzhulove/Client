using System;
using System.Text;

namespace Mock.Protocol
{

	public class DailyTodoInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		/// 唯一id
		/// </summary>
		[AdvancedInspector.Descriptor("唯一id", "唯一id")]
		public UInt64 id;
		/// <summary>
		/// 表id
		/// </summary>
		[AdvancedInspector.Descriptor("表id", "表id")]
		public UInt32 dataId;
		/// <summary>
		/// 每日进度
		/// </summary>
		[AdvancedInspector.Descriptor("每日进度", "每日进度")]
		public UInt32 dayProgress;
		/// <summary>
		/// 每日进度更新时间戳
		/// </summary>
		[AdvancedInspector.Descriptor("每日进度更新时间戳", "每日进度更新时间戳")]
		public UInt32 dayProgTime;
		/// <summary>
		/// 每日进度最大值
		/// </summary>
		[AdvancedInspector.Descriptor("每日进度最大值", "每日进度最大值")]
		public UInt32 dayProgMax;
		/// <summary>
		/// 每周进度
		/// </summary>
		[AdvancedInspector.Descriptor("每周进度", "每周进度")]
		public UInt32 weekProgress;
		/// <summary>
		/// 每周进度更新时间戳
		/// </summary>
		[AdvancedInspector.Descriptor("每周进度更新时间戳", "每周进度更新时间戳")]
		public UInt32 weekProgTime;
		/// <summary>
		/// 每周进度最大值
		/// </summary>
		[AdvancedInspector.Descriptor("每周进度最大值", "每周进度最大值")]
		public UInt32 weekProgMax;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, id);
			BaseDLL.encode_uint32(buffer, ref pos_, dataId);
			BaseDLL.encode_uint32(buffer, ref pos_, dayProgress);
			BaseDLL.encode_uint32(buffer, ref pos_, dayProgTime);
			BaseDLL.encode_uint32(buffer, ref pos_, dayProgMax);
			BaseDLL.encode_uint32(buffer, ref pos_, weekProgress);
			BaseDLL.encode_uint32(buffer, ref pos_, weekProgTime);
			BaseDLL.encode_uint32(buffer, ref pos_, weekProgMax);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref id);
			BaseDLL.decode_uint32(buffer, ref pos_, ref dataId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref dayProgress);
			BaseDLL.decode_uint32(buffer, ref pos_, ref dayProgTime);
			BaseDLL.decode_uint32(buffer, ref pos_, ref dayProgMax);
			BaseDLL.decode_uint32(buffer, ref pos_, ref weekProgress);
			BaseDLL.decode_uint32(buffer, ref pos_, ref weekProgTime);
			BaseDLL.decode_uint32(buffer, ref pos_, ref weekProgMax);
		}


		#endregion

	}

}
