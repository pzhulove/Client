using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  返回公会仓库列表
	/// </summary>
	[AdvancedInspector.Descriptor(" 返回公会仓库列表", " 返回公会仓库列表")]
	public class WorldGuildStorageListRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601970;
		public UInt32 Sequence;

		public UInt32 result;

		public UInt32 maxSize;

		public GuildStorageItemInfo[] items = null;

		public GuildStorageOpRecord[] itemRecords = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, result);
			BaseDLL.encode_uint32(buffer, ref pos_, maxSize);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)items.Length);
			for(int i = 0; i < items.Length; i++)
			{
				items[i].encode(buffer, ref pos_);
			}
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)itemRecords.Length);
			for(int i = 0; i < itemRecords.Length; i++)
			{
				itemRecords[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			BaseDLL.decode_uint32(buffer, ref pos_, ref maxSize);
			UInt16 itemsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref itemsCnt);
			items = new GuildStorageItemInfo[itemsCnt];
			for(int i = 0; i < items.Length; i++)
			{
				items[i] = new GuildStorageItemInfo();
				items[i].decode(buffer, ref pos_);
			}
			UInt16 itemRecordsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref itemRecordsCnt);
			itemRecords = new GuildStorageOpRecord[itemRecordsCnt];
			for(int i = 0; i < itemRecords.Length; i++)
			{
				itemRecords[i] = new GuildStorageOpRecord();
				itemRecords[i].decode(buffer, ref pos_);
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
