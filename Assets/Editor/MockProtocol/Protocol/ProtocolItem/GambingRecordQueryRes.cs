using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// world->client 夺宝记录查询返回
	/// </summary>
	[AdvancedInspector.Descriptor("world->client 夺宝记录查询返回", "world->client 夺宝记录查询返回")]
	public class GambingRecordQueryRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 707908;
		public UInt32 Sequence;

		public GambingItemRecordData[] gambingRecordDatas = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)gambingRecordDatas.Length);
			for(int i = 0; i < gambingRecordDatas.Length; i++)
			{
				gambingRecordDatas[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 gambingRecordDatasCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref gambingRecordDatasCnt);
			gambingRecordDatas = new GambingItemRecordData[gambingRecordDatasCnt];
			for(int i = 0; i < gambingRecordDatas.Length; i++)
			{
				gambingRecordDatas[i] = new GambingItemRecordData();
				gambingRecordDatas[i].decode(buffer, ref pos_);
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
