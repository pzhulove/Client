using System;
using System.Text;

namespace Mock.Protocol
{

	public class WorldGetItemFriendPresentInfosRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 609702;
		public UInt32 Sequence;
		/// <summary>
		///  道具id
		/// </summary>
		[AdvancedInspector.Descriptor(" 道具id", " 道具id")]
		public UInt32 dataId;
		/// <summary>
		///  赠送数据
		/// </summary>
		[AdvancedInspector.Descriptor(" 赠送数据", " 赠送数据")]
		public FriendPresentInfo[] presentInfos = null;
		/// <summary>
		///  被赠送总次数
		/// </summary>
		[AdvancedInspector.Descriptor(" 被赠送总次数", " 被赠送总次数")]
		public UInt32 recvedTotal;
		/// <summary>
		///  被赠送总次数上限
		/// </summary>
		[AdvancedInspector.Descriptor(" 被赠送总次数上限", " 被赠送总次数上限")]
		public UInt32 recvedTotalLimit;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, dataId);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)presentInfos.Length);
			for(int i = 0; i < presentInfos.Length; i++)
			{
				presentInfos[i].encode(buffer, ref pos_);
			}
			BaseDLL.encode_uint32(buffer, ref pos_, recvedTotal);
			BaseDLL.encode_uint32(buffer, ref pos_, recvedTotalLimit);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref dataId);
			UInt16 presentInfosCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref presentInfosCnt);
			presentInfos = new FriendPresentInfo[presentInfosCnt];
			for(int i = 0; i < presentInfos.Length; i++)
			{
				presentInfos[i] = new FriendPresentInfo();
				presentInfos[i].decode(buffer, ref pos_);
			}
			BaseDLL.decode_uint32(buffer, ref pos_, ref recvedTotal);
			BaseDLL.decode_uint32(buffer, ref pos_, ref recvedTotalLimit);
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
