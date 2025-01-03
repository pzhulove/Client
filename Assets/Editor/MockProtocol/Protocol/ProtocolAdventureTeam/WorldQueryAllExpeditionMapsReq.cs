using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  client->world 查询全部远征地图请求
	/// </summary>
	[AdvancedInspector.Descriptor(" client->world 查询全部远征地图请求", " client->world 查询全部远征地图请求")]
	public class WorldQueryAllExpeditionMapsReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 608621;
		public UInt32 Sequence;
		/// <summary>
		///  地图id集
		/// </summary>
		[AdvancedInspector.Descriptor(" 地图id集", " 地图id集")]
		public byte[] mapIds = new byte[0];

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)mapIds.Length);
			for(int i = 0; i < mapIds.Length; i++)
			{
				BaseDLL.encode_int8(buffer, ref pos_, mapIds[i]);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 mapIdsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref mapIdsCnt);
			mapIds = new byte[mapIdsCnt];
			for(int i = 0; i < mapIds.Length; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref mapIds[i]);
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
