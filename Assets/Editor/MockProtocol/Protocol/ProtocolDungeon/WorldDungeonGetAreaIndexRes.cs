using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  地下城房间索引返回
	/// </summary>
	[AdvancedInspector.Descriptor(" 地下城房间索引返回", " 地下城房间索引返回")]
	public class WorldDungeonGetAreaIndexRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 606816;
		public UInt32 Sequence;
		/// <summary>
		///  地下城ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 地下城ID", " 地下城ID")]
		public UInt32 dungeonId;
		/// <summary>
		///  房间索引
		/// </summary>
		[AdvancedInspector.Descriptor(" 房间索引", " 房间索引")]
		public UInt32 areaIndex;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, dungeonId);
			BaseDLL.encode_uint32(buffer, ref pos_, areaIndex);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref dungeonId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref areaIndex);
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
