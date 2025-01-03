using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 查询商城道具返回
	/// </summary>
	[AdvancedInspector.Descriptor("查询商城道具返回", "查询商城道具返回")]
	public class WorldMallQueryItemRet : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 602804;
		public UInt32 Sequence;
		/// <summary>
		/// 商城主页签
		/// </summary>
		[AdvancedInspector.Descriptor("商城主页签", "商城主页签")]
		public byte type;

		public MallItemInfo[] items = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, type);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)items.Length);
			for(int i = 0; i < items.Length; i++)
			{
				items[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref type);
			UInt16 itemsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref itemsCnt);
			items = new MallItemInfo[itemsCnt];
			for(int i = 0; i < items.Length; i++)
			{
				items[i] = new MallItemInfo();
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
