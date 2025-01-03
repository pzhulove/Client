using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  请求初始化技能
	/// </summary>
	[AdvancedInspector.Descriptor(" 请求初始化技能", " 请求初始化技能")]
	public class SceneInitSkillsReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 500713;
		public UInt32 Sequence;
		/// <summary>
		///  技能配置类型
		/// </summary>
		[AdvancedInspector.Descriptor(" 技能配置类型", " 技能配置类型")]
		public UInt32 configType;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, configType);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref configType);
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
