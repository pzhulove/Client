using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  升级技能
	/// </summary>
	[AdvancedInspector.Descriptor(" 升级技能", " 升级技能")]
	public class WorldGuildUpgradeSkill : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601931;
		public UInt32 Sequence;
		/// <summary>
		///  技能id
		/// </summary>
		[AdvancedInspector.Descriptor(" 技能id", " 技能id")]
		public UInt16 skillId;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, skillId);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint16(buffer, ref pos_, ref skillId);
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
