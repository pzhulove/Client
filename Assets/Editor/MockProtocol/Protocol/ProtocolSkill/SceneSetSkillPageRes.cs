using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  设置技能页返回
	/// </summary>
	[AdvancedInspector.Descriptor(" 设置技能页返回", " 设置技能页返回")]
	public class SceneSetSkillPageRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 500719;
		public UInt32 Sequence;
		/// <summary>
		///  技能配置类型
		/// </summary>
		[AdvancedInspector.Descriptor(" 技能配置类型", " 技能配置类型")]
		public UInt32 configType;
		/// <summary>
		/// 修改的技能页
		/// </summary>
		[AdvancedInspector.Descriptor("修改的技能页", "修改的技能页")]
		public byte page;
		/// <summary>
		///  返回值
		/// </summary>
		[AdvancedInspector.Descriptor(" 返回值", " 返回值")]
		public UInt32 result;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, configType);
			BaseDLL.encode_int8(buffer, ref pos_, page);
			BaseDLL.encode_uint32(buffer, ref pos_, result);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref configType);
			BaseDLL.decode_int8(buffer, ref pos_, ref page);
			BaseDLL.decode_uint32(buffer, ref pos_, ref result);
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
