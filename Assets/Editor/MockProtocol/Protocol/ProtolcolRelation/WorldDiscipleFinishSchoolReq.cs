using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// client->world 请求徒弟出师
	/// </summary>
	[AdvancedInspector.Descriptor("client->world 请求徒弟出师", "client->world 请求徒弟出师")]
	public class WorldDiscipleFinishSchoolReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601749;
		public UInt32 Sequence;

		public UInt64 discipleId;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, discipleId);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref discipleId);
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
