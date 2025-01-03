using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  选择技能返回
	/// </summary>
	[AdvancedInspector.Descriptor(" 选择技能返回", " 选择技能返回")]
	public class BattleChoiceSkillRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 508941;
		public UInt32 Sequence;

		public UInt32 retCode;

		public UInt32 skillId;

		public UInt32 skillLvl;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, retCode);
			BaseDLL.encode_uint32(buffer, ref pos_, skillId);
			BaseDLL.encode_uint32(buffer, ref pos_, skillLvl);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
			BaseDLL.decode_uint32(buffer, ref pos_, ref skillId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref skillLvl);
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
