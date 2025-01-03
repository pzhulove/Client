using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// client->world ͽ�ܻ㱨ʦ�������������
	/// </summary>
	[AdvancedInspector.Descriptor("client->world ͽ�ܻ㱨ʦ�������������", "client->world ͽ�ܻ㱨ʦ�������������")]
	public class WorldReportMasterDailyTaskReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601761;
		public UInt32 Sequence;

		public UInt64 masterId;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, masterId);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref masterId);
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
