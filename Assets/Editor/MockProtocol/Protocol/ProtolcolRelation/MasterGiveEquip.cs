using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 是否收徒
	/// </summary>
	/// <summary>
	/// 师傅赠送装备
	/// </summary>
	[AdvancedInspector.Descriptor("师傅赠送装备", "师傅赠送装备")]
	public class MasterGiveEquip : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501701;
		public UInt32 Sequence;

		public UInt64[] itemUids = new UInt64[0];
		/// <summary>
		/// 装备uid
		/// </summary>
		[AdvancedInspector.Descriptor("装备uid", "装备uid")]
		public UInt64 discipleId;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)itemUids.Length);
			for(int i = 0; i < itemUids.Length; i++)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, itemUids[i]);
			}
			BaseDLL.encode_uint64(buffer, ref pos_, discipleId);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 itemUidsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref itemUidsCnt);
			itemUids = new UInt64[itemUidsCnt];
			for(int i = 0; i < itemUids.Length; i++)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref itemUids[i]);
			}
			BaseDLL.decode_uint64(buffer, ref pos_, ref discipleId);
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
