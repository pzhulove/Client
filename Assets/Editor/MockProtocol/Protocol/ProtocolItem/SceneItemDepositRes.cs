using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  物品寄存返回
	/// </summary>
	[AdvancedInspector.Descriptor(" 物品寄存返回", " 物品寄存返回")]
	public class SceneItemDepositRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501051;
		public UInt32 Sequence;
		/// <summary>
		///  过期时间
		/// </summary>
		[AdvancedInspector.Descriptor(" 过期时间", " 过期时间")]
		public UInt32 expireTime;
		/// <summary>
		///  物品列表
		/// </summary>
		[AdvancedInspector.Descriptor(" 物品列表", " 物品列表")]
		public depositItem[] depositItemList = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, expireTime);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)depositItemList.Length);
			for(int i = 0; i < depositItemList.Length; i++)
			{
				depositItemList[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref expireTime);
			UInt16 depositItemListCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref depositItemListCnt);
			depositItemList = new depositItem[depositItemListCnt];
			for(int i = 0; i < depositItemList.Length; i++)
			{
				depositItemList[i] = new depositItem();
				depositItemList[i].decode(buffer, ref pos_);
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
