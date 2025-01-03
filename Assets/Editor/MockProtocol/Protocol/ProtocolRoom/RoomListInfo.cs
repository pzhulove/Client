using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 房间列表信息
	/// </summary>
	[AdvancedInspector.Descriptor("房间列表信息", "房间列表信息")]
	public class RoomListInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt32 startIndex;

		public UInt32 total;

		public RoomSimpleInfo[] rooms = null;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, startIndex);
			BaseDLL.encode_uint32(buffer, ref pos_, total);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)rooms.Length);
			for(int i = 0; i < rooms.Length; i++)
			{
				rooms[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref startIndex);
			BaseDLL.decode_uint32(buffer, ref pos_, ref total);
			UInt16 roomsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref roomsCnt);
			rooms = new RoomSimpleInfo[roomsCnt];
			for(int i = 0; i < rooms.Length; i++)
			{
				rooms[i] = new RoomSimpleInfo();
				rooms[i].decode(buffer, ref pos_);
			}
		}


		#endregion

	}

}
