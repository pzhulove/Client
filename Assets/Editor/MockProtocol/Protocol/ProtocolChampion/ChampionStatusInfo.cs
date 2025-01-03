using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 冠军赛状态信息
	/// </summary>
	[AdvancedInspector.Descriptor("冠军赛状态信息", "冠军赛状态信息")]
	public class ChampionStatusInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt32 status;

		public UInt32 startTime;

		public UInt32 endTime;

		public UInt32 preStartTime;

		public UInt32 prepareStartTime;

		public UInt32 battleStartTime;

		public UInt32 battleEndTime;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, status);
			BaseDLL.encode_uint32(buffer, ref pos_, startTime);
			BaseDLL.encode_uint32(buffer, ref pos_, endTime);
			BaseDLL.encode_uint32(buffer, ref pos_, preStartTime);
			BaseDLL.encode_uint32(buffer, ref pos_, prepareStartTime);
			BaseDLL.encode_uint32(buffer, ref pos_, battleStartTime);
			BaseDLL.encode_uint32(buffer, ref pos_, battleEndTime);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref status);
			BaseDLL.decode_uint32(buffer, ref pos_, ref startTime);
			BaseDLL.decode_uint32(buffer, ref pos_, ref endTime);
			BaseDLL.decode_uint32(buffer, ref pos_, ref preStartTime);
			BaseDLL.decode_uint32(buffer, ref pos_, ref prepareStartTime);
			BaseDLL.decode_uint32(buffer, ref pos_, ref battleStartTime);
			BaseDLL.decode_uint32(buffer, ref pos_, ref battleEndTime);
		}


		#endregion

	}

}
