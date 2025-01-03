using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// *****************************************************************
	/// </summary>
	/// <summary>
	///  玩家押注
	/// </summary>
	[AdvancedInspector.Descriptor(" 玩家押注", " 玩家押注")]
	public class stakeReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 708307;
		public UInt32 Sequence;
		/// <summary>
		///  押注的射手
		/// </summary>
		[AdvancedInspector.Descriptor(" 押注的射手", " 押注的射手")]
		public UInt32 id;
		/// <summary>
		///  押注子弹数量
		/// </summary>
		[AdvancedInspector.Descriptor(" 押注子弹数量", " 押注子弹数量")]
		public UInt32 num;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, id);
			BaseDLL.encode_uint32(buffer, ref pos_, num);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref id);
			BaseDLL.decode_uint32(buffer, ref pos_, ref num);
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
