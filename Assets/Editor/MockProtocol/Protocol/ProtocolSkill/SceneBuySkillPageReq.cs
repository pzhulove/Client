using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  购买技能页
	/// </summary>
	[AdvancedInspector.Descriptor(" 购买技能页", " 购买技能页")]
	public class SceneBuySkillPageReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 500720;
		public UInt32 Sequence;
		/// <summary>
		///  技能配置类型
		/// </summary>
		[AdvancedInspector.Descriptor(" 技能配置类型", " 技能配置类型")]
		public UInt32 configType;
		/// <summary>
		/// 购买的技能页
		/// </summary>
		[AdvancedInspector.Descriptor("购买的技能页", "购买的技能页")]
		public byte page;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, configType);
			BaseDLL.encode_int8(buffer, ref pos_, page);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref configType);
			BaseDLL.decode_int8(buffer, ref pos_, ref page);
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
