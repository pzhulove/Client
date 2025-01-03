using System;
using System.Text;

namespace Mock.Protocol
{

	public class SceneBattleNotifySpoilsItem : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 508915;
		public UInt32 Sequence;

		public UInt32 battleID;

		public UInt64 playerID;

		public DetailItem[] detailItems = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, battleID);
			BaseDLL.encode_uint64(buffer, ref pos_, playerID);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)detailItems.Length);
			for(int i = 0; i < detailItems.Length; i++)
			{
				detailItems[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref battleID);
			BaseDLL.decode_uint64(buffer, ref pos_, ref playerID);
			UInt16 detailItemsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref detailItemsCnt);
			detailItems = new DetailItem[detailItemsCnt];
			for(int i = 0; i < detailItems.Length; i++)
			{
				detailItems[i] = new DetailItem();
				detailItems[i].decode(buffer, ref pos_);
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
