using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  返回结算掉落
	/// </summary>
	[AdvancedInspector.Descriptor(" 返回结算掉落", " 返回结算掉落")]
	public class SceneDungeonEndDropRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 506824;
		public UInt32 Sequence;
		/// <summary>
		///  总倍率（0代表获取失败）
		/// </summary>
		[AdvancedInspector.Descriptor(" 总倍率（0代表获取失败）", " 总倍率（0代表获取失败）")]
		public byte multi;
		/// <summary>
		///  掉落物品
		/// </summary>
		[AdvancedInspector.Descriptor(" 掉落物品", " 掉落物品")]
		public SceneDungeonDropItem[] items = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, multi);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)items.Length);
			for(int i = 0; i < items.Length; i++)
			{
				items[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref multi);
			UInt16 itemsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref itemsCnt);
			items = new SceneDungeonDropItem[itemsCnt];
			for(int i = 0; i < items.Length; i++)
			{
				items[i] = new SceneDungeonDropItem();
				items[i].decode(buffer, ref pos_);
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
