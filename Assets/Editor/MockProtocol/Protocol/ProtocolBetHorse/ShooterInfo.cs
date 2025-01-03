using System;
using System.Text;

namespace Mock.Protocol
{

	public class ShooterInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  射手id(唯一id)
		/// </summary>
		[AdvancedInspector.Descriptor(" 射手id(唯一id)", " 射手id(唯一id)")]
		public UInt32 id;
		/// <summary>
		///  dataid
		/// </summary>
		[AdvancedInspector.Descriptor(" dataid", " dataid")]
		public UInt32 dataid;
		/// <summary>
		///  状态(ShooterStatusType)
		/// </summary>
		[AdvancedInspector.Descriptor(" 状态(ShooterStatusType)", " 状态(ShooterStatusType)")]
		public UInt32 status;
		/// <summary>
		///  赔率
		/// </summary>
		[AdvancedInspector.Descriptor(" 赔率", " 赔率")]
		public UInt32 odds;
		/// <summary>
		///  胜率
		/// </summary>
		[AdvancedInspector.Descriptor(" 胜率", " 胜率")]
		public UInt32 winRate;
		/// <summary>
		///  吃鸡数(夺冠次数)
		/// </summary>
		[AdvancedInspector.Descriptor(" 吃鸡数(夺冠次数)", " 吃鸡数(夺冠次数)")]
		public UInt32 champion;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, id);
			BaseDLL.encode_uint32(buffer, ref pos_, dataid);
			BaseDLL.encode_uint32(buffer, ref pos_, status);
			BaseDLL.encode_uint32(buffer, ref pos_, odds);
			BaseDLL.encode_uint32(buffer, ref pos_, winRate);
			BaseDLL.encode_uint32(buffer, ref pos_, champion);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref id);
			BaseDLL.decode_uint32(buffer, ref pos_, ref dataid);
			BaseDLL.decode_uint32(buffer, ref pos_, ref status);
			BaseDLL.decode_uint32(buffer, ref pos_, ref odds);
			BaseDLL.decode_uint32(buffer, ref pos_, ref winRate);
			BaseDLL.decode_uint32(buffer, ref pos_, ref champion);
		}


		#endregion

	}

}
