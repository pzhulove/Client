using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  同步仓库物品数据
	/// </summary>
	[AdvancedInspector.Descriptor(" 同步仓库物品数据", " 同步仓库物品数据")]
	public class WorldGuildStorageItemSync : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601971;
		public UInt32 Sequence;

		public GuildStorageItemInfo[] items = null;

		public GuildStorageOpRecord[] records = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)items.Length);
			for(int i = 0; i < items.Length; i++)
			{
				items[i].encode(buffer, ref pos_);
			}
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)records.Length);
			for(int i = 0; i < records.Length; i++)
			{
				records[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 itemsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref itemsCnt);
			items = new GuildStorageItemInfo[itemsCnt];
			for(int i = 0; i < items.Length; i++)
			{
				items[i] = new GuildStorageItemInfo();
				items[i].decode(buffer, ref pos_);
			}
			UInt16 recordsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref recordsCnt);
			records = new GuildStorageOpRecord[recordsCnt];
			for(int i = 0; i < records.Length; i++)
			{
				records[i] = new GuildStorageOpRecord();
				records[i].decode(buffer, ref pos_);
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
