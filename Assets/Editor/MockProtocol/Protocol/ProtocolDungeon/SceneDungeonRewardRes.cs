using System;
using System.Text;

namespace Mock.Protocol
{

	public class SceneDungeonRewardRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 506810;
		public UInt32 Sequence;

		public UInt32[] pickedItems = new UInt32[0];

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)pickedItems.Length);
			for(int i = 0; i < pickedItems.Length; i++)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, pickedItems[i]);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 pickedItemsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref pickedItemsCnt);
			pickedItems = new UInt32[pickedItemsCnt];
			for(int i = 0; i < pickedItems.Length; i++)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref pickedItems[i]);
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
