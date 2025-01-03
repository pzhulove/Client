using System;
using System.Text;

namespace Mock.Protocol
{

	public class SceneDungeonAddMonsterDropItemNotify : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 506815;
		public UInt32 Sequence;

		public UInt32 monsterId;

		public SceneDungeonDropItem[] dropItems = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, monsterId);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)dropItems.Length);
			for(int i = 0; i < dropItems.Length; i++)
			{
				dropItems[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref monsterId);
			UInt16 dropItemsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref dropItemsCnt);
			dropItems = new SceneDungeonDropItem[dropItemsCnt];
			for(int i = 0; i < dropItems.Length; i++)
			{
				dropItems[i] = new SceneDungeonDropItem();
				dropItems[i].decode(buffer, ref pos_);
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
