using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// client->world	 自动出师请求
	/// </summary>
	[AdvancedInspector.Descriptor("client->world	 自动出师请求", "client->world	 自动出师请求")]
	public class WorldAutomaticFinishSchoolReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601772;
		public UInt32 Sequence;

		public UInt64 targetId;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, targetId);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref targetId);
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
