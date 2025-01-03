using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// client->world 装备回收开罐子记录请求
	/// </summary>
	[AdvancedInspector.Descriptor("client->world 装备回收开罐子记录请求", "client->world 装备回收开罐子记录请求")]
	public class WorldEquipRecoOpenJarsRecordReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 600904;
		public UInt32 Sequence;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
		}

		public void decode(byte[] buffer, ref int pos_)
		{
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
