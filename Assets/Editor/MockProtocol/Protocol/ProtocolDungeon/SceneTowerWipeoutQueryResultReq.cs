using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  请求死亡之塔扫荡奖励（指定层数）
	/// </summary>
	[AdvancedInspector.Descriptor(" 请求死亡之塔扫荡奖励（指定层数）", " 请求死亡之塔扫荡奖励（指定层数）")]
	public class SceneTowerWipeoutQueryResultReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 507211;
		public UInt32 Sequence;
		/// <summary>
		/// 起始层数
		/// </summary>
		[AdvancedInspector.Descriptor("起始层数", "起始层数")]
		public UInt32 beginFloor;
		/// <summary>
		/// 结束层数
		/// </summary>
		[AdvancedInspector.Descriptor("结束层数", "结束层数")]
		public UInt32 endFloor;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, beginFloor);
			BaseDLL.encode_uint32(buffer, ref pos_, endFloor);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref beginFloor);
			BaseDLL.decode_uint32(buffer, ref pos_, ref endFloor);
		}

		public UInt32 GetSequence()
		{
			return Sequence;
		}

		public void SetSequence(UInt32 sequence)
		{
			Sequence = sequence;
		}

		#endregion

	}

}
