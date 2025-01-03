using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// world->client ͽ����ȡʦ�ųɳ���������
	/// </summary>
	[AdvancedInspector.Descriptor("world->client ͽ����ȡʦ�ųɳ���������", "world->client ͽ����ȡʦ�ųɳ���������")]
	public class WorldReceiveMasterAcademicTaskRewardRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601768;
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
