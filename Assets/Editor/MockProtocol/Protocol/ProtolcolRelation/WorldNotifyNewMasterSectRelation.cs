using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// world->client 通知新同门关系
	/// </summary>
	/// <summary>
	/// 格式 id(ObjID_t) + data
	/// </summary>
	[AdvancedInspector.Descriptor("格式 id(ObjID_t) + data", "格式 id(ObjID_t) + data")]
	public class WorldNotifyNewMasterSectRelation : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601743;
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
