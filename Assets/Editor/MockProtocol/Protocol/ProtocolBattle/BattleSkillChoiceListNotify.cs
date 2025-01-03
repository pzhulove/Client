using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  下发技能列表
	/// </summary>
	[AdvancedInspector.Descriptor(" 下发技能列表", " 下发技能列表")]
	public class BattleSkillChoiceListNotify : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 508939;
		public UInt32 Sequence;

		public ChiJiSkill[] skillList = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)skillList.Length);
			for(int i = 0; i < skillList.Length; i++)
			{
				skillList[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 skillListCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref skillListCnt);
			skillList = new ChiJiSkill[skillListCnt];
			for(int i = 0; i < skillList.Length; i++)
			{
				skillList[i] = new ChiJiSkill();
				skillList[i].decode(buffer, ref pos_);
			}
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
