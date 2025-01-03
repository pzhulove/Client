using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 请求商城礼包详情返回
	/// </summary>
	[AdvancedInspector.Descriptor("请求商城礼包详情返回", "请求商城礼包详情返回")]
	public class WorldMallQueryItemDetailRet : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 602806;
		public UInt32 Sequence;

		public MallGiftDetail[] details = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)details.Length);
			for(int i = 0; i < details.Length; i++)
			{
				details[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 detailsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref detailsCnt);
			details = new MallGiftDetail[detailsCnt];
			for(int i = 0; i < details.Length; i++)
			{
				details[i] = new MallGiftDetail();
				details[i].decode(buffer, ref pos_);
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
