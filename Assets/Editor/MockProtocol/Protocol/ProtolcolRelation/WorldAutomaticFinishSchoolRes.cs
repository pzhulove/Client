using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 目标角色id
	/// </summary>
	/// <summary>
	/// world->client	 自动出师返回
	/// </summary>
	[AdvancedInspector.Descriptor("world->client	 自动出师返回", "world->client	 自动出师返回")]
	public class WorldAutomaticFinishSchoolRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601773;
		public UInt32 Sequence;

		public UInt32 code;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, code);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref code);
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
