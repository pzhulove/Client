using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// scene->client 同步楼层数据 
	/// </summary>
	[AdvancedInspector.Descriptor("scene->client 同步楼层数据 ", "scene->client 同步楼层数据 ")]
	public class LostDungeonSyncFloor : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 510018;
		public UInt32 Sequence;

		public LostDungeonFloorData[] floorData = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)floorData.Length);
			for(int i = 0; i < floorData.Length; i++)
			{
				floorData[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 floorDataCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref floorDataCnt);
			floorData = new LostDungeonFloorData[floorDataCnt];
			for(int i = 0; i < floorData.Length; i++)
			{
				floorData[i] = new LostDungeonFloorData();
				floorData[i].decode(buffer, ref pos_);
			}
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
