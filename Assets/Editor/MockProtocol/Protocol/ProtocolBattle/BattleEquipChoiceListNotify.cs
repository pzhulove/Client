using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  下发装备选择列表
	/// </summary>
	[AdvancedInspector.Descriptor(" 下发装备选择列表", " 下发装备选择列表")]
	public class BattleEquipChoiceListNotify : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 508949;
		public UInt32 Sequence;

		public UInt32[] equipList = new UInt32[0];

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)equipList.Length);
			for(int i = 0; i < equipList.Length; i++)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, equipList[i]);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 equipListCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref equipListCnt);
			equipList = new UInt32[equipListCnt];
			for(int i = 0; i < equipList.Length; i++)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref equipList[i]);
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
