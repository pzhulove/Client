using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 激活条件
	/// </summary>
	/// <summary>
	/// 激活商城限时礼包返回
	/// </summary>
	[AdvancedInspector.Descriptor("激活商城限时礼包返回", "激活商城限时礼包返回")]
	public class WorldMallGiftPackActivateRet : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 602815;
		public UInt32 Sequence;

		public MallItemInfo[] items = null;
		/// <summary>
		/// 一个礼包
		/// </summary>
		[AdvancedInspector.Descriptor("一个礼包", "一个礼包")]
		public UInt32 code;

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
			BaseDLL.encode_uint32(buffer, ref pos_, code);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 itemsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref itemsCnt);
			items = new MallItemInfo[itemsCnt];
			for(int i = 0; i < items.Length; i++)
			{
				items[i] = new MallItemInfo();
				items[i].decode(buffer, ref pos_);
			}
			BaseDLL.decode_uint32(buffer, ref pos_, ref code);
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
