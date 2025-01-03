using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// world->client 上线同步同门关系列表
	/// </summary>
	/// <summary>
	/// datalist格式: ObjID_t + isOnline(UInt8) + data + .. + 0(ObjID_t)
	/// </summary>
	[AdvancedInspector.Descriptor("datalist格式: ObjID_t + isOnline(UInt8) + data + .. + 0(ObjID_t)", "datalist格式: ObjID_t + isOnline(UInt8) + data + .. + 0(ObjID_t)")]
	public class WorldSyncMasterSectRelationList : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601745;
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
