using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  战场报名返回
	/// </summary>
	[AdvancedInspector.Descriptor(" 战场报名返回", " 战场报名返回")]
	public class BattleEnrollRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 2200006;
		public UInt32 Sequence;
		/// <summary>
		///  0取消报名，非0报名
		/// </summary>
		[AdvancedInspector.Descriptor(" 0取消报名，非0报名", " 0取消报名，非0报名")]
		public UInt32 isMatch;
		/// <summary>
		///  返回值
		/// </summary>
		[AdvancedInspector.Descriptor(" 返回值", " 返回值")]
		public UInt32 retCode;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, isMatch);
			BaseDLL.encode_uint32(buffer, ref pos_, retCode);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref isMatch);
			BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
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
