using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  设置公平竞技技能配置请求 0表示查询设置情况 1表示第一次设置
	/// </summary>
	[AdvancedInspector.Descriptor(" 设置公平竞技技能配置请求 0表示查询设置情况 1表示第一次设置", " 设置公平竞技技能配置请求 0表示查询设置情况 1表示第一次设置")]
	public class SceneSetEqualPvpSkillConfigReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501226;
		public UInt32 Sequence;

		public byte isSetedEqualPvPConfig;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, isSetedEqualPvPConfig);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref isSetedEqualPvPConfig);
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
