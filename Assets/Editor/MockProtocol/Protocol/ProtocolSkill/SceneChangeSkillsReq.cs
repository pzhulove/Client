using System;
using System.Text;

namespace Mock.Protocol
{

	public class SceneChangeSkillsReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 500701;
		public UInt32 Sequence;

		public byte configType;

		public ChangeSkill[] skills = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, configType);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)skills.Length);
			for(int i = 0; i < skills.Length; i++)
			{
				skills[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref configType);
			UInt16 skillsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref skillsCnt);
			skills = new ChangeSkill[skillsCnt];
			for(int i = 0; i < skills.Length; i++)
			{
				skills[i] = new ChangeSkill();
				skills[i].decode(buffer, ref pos_);
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
