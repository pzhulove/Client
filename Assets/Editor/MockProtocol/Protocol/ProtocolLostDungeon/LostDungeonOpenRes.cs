using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 开启迷失地牢返回
	/// </summary>
	[AdvancedInspector.Descriptor("开启迷失地牢返回", "开启迷失地牢返回")]
	public class LostDungeonOpenRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 510004;
		public UInt32 Sequence;
		/// <summary>
		/// 结果
		/// </summary>
		[AdvancedInspector.Descriptor("结果", "结果")]
		public UInt32 code;
		/// <summary>
		/// 楼层数据
		/// </summary>
		[AdvancedInspector.Descriptor("楼层数据", "楼层数据")]
		public LostDungeonFloorData[] floorDatas = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, code);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)floorDatas.Length);
			for(int i = 0; i < floorDatas.Length; i++)
			{
				floorDatas[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			UInt16 floorDatasCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref floorDatasCnt);
			floorDatas = new LostDungeonFloorData[floorDatasCnt];
			for(int i = 0; i < floorDatas.Length; i++)
			{
				floorDatas[i] = new LostDungeonFloorData();
				floorDatas[i].decode(buffer, ref pos_);
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
