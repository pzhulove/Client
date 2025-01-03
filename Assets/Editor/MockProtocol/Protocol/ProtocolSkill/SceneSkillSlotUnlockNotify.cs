using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  技能槽解锁
	/// </summary>
	[AdvancedInspector.Descriptor(" 技能槽解锁", " 技能槽解锁")]
	public class SceneSkillSlotUnlockNotify : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 500717;
		public UInt32 Sequence;
		/// <summary>
		///  槽位
		/// </summary>
		[AdvancedInspector.Descriptor(" 槽位", " 槽位")]
		public UInt32 slot;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, slot);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref slot);
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
