using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// client->world  ��ȡʦ���ճ�������ɽ�������
	/// </summary>
	[AdvancedInspector.Descriptor("client->world  ��ȡʦ���ճ�������ɽ�������", "client->world  ��ȡʦ���ճ�������ɽ�������")]
	public class WorldReceiveMasterDailyTaskRewardReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601765;
		public UInt32 Sequence;

		public byte type;

		public UInt64 discipeleId;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, type);
			BaseDLL.encode_uint64(buffer, ref pos_, discipeleId);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref type);
			BaseDLL.decode_uint64(buffer, ref pos_, ref discipeleId);
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
