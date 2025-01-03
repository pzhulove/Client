using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 押注项目信息
	/// </summary>
	[AdvancedInspector.Descriptor("押注项目信息", "押注项目信息")]
	public class GambleInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		/// 项目id
		/// </summary>
		[AdvancedInspector.Descriptor("项目id", "项目id")]
		public UInt32 id;
		/// <summary>
		/// 项目类型
		/// </summary>
		[AdvancedInspector.Descriptor("项目类型", "项目类型")]
		public byte type;
		/// <summary>
		/// 本项目所有的选项
		/// </summary>
		[AdvancedInspector.Descriptor("本项目所有的选项", "本项目所有的选项")]
		public GambleOption[] options = null;
		/// <summary>
		/// 开始时间
		/// </summary>
		[AdvancedInspector.Descriptor("开始时间", "开始时间")]
		public UInt32 startTime;
		/// <summary>
		/// 截至时间
		/// </summary>
		[AdvancedInspector.Descriptor("截至时间", "截至时间")]
		public UInt32 endTime;
		/// <summary>
		/// 参数 类型为单场时表示组id
		/// </summary>
		[AdvancedInspector.Descriptor("参数 类型为单场时表示组id", "参数 类型为单场时表示组id")]
		public UInt64 param;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, id);
			BaseDLL.encode_int8(buffer, ref pos_, type);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)options.Length);
			for(int i = 0; i < options.Length; i++)
			{
				options[i].encode(buffer, ref pos_);
			}
			BaseDLL.encode_uint32(buffer, ref pos_, startTime);
			BaseDLL.encode_uint32(buffer, ref pos_, endTime);
			BaseDLL.encode_uint64(buffer, ref pos_, param);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref id);
			BaseDLL.decode_int8(buffer, ref pos_, ref type);
			UInt16 optionsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref optionsCnt);
			options = new GambleOption[optionsCnt];
			for(int i = 0; i < options.Length; i++)
			{
				options[i] = new GambleOption();
				options[i].decode(buffer, ref pos_);
			}
			BaseDLL.decode_uint32(buffer, ref pos_, ref startTime);
			BaseDLL.decode_uint32(buffer, ref pos_, ref endTime);
			BaseDLL.decode_uint64(buffer, ref pos_, ref param);
		}


		#endregion

	}

}
