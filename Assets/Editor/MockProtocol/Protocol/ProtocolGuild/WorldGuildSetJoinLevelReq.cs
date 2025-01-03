using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  设置加入公会等级请求
	/// </summary>
	[AdvancedInspector.Descriptor(" 设置加入公会等级请求", " 设置加入公会等级请求")]
	public class WorldGuildSetJoinLevelReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601988;
		public UInt32 Sequence;
		/// <summary>
		///  加入等级
		/// </summary>
		[AdvancedInspector.Descriptor(" 加入等级", " 加入等级")]
		public UInt32 joinLevel;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, joinLevel);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref joinLevel);
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
