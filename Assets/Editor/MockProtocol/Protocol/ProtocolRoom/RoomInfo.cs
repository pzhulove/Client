using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 房间信息
	/// </summary>
	[AdvancedInspector.Descriptor("房间信息", "房间信息")]
	public class RoomInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public RoomSimpleInfo roomSimpleInfo = null;

		public RoomSlotInfo[] roomSlotInfos = null;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			roomSimpleInfo.encode(buffer, ref pos_);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)roomSlotInfos.Length);
			for(int i = 0; i < roomSlotInfos.Length; i++)
			{
				roomSlotInfos[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			roomSimpleInfo.decode(buffer, ref pos_);
			UInt16 roomSlotInfosCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref roomSlotInfosCnt);
			roomSlotInfos = new RoomSlotInfo[roomSlotInfosCnt];
			for(int i = 0; i < roomSlotInfos.Length; i++)
			{
				roomSlotInfos[i] = new RoomSlotInfo();
				roomSlotInfos[i].decode(buffer, ref pos_);
			}
		}


		#endregion

	}

}
