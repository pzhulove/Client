using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// world->client 我的夺宝数据查询返回
	/// </summary>
	[AdvancedInspector.Descriptor("world->client 我的夺宝数据查询返回", "world->client 我的夺宝数据查询返回")]
	public class GambingMineQueryRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 707906;
		public UInt32 Sequence;

		public GambingMineInfo[] mineGambingInfo = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)mineGambingInfo.Length);
			for(int i = 0; i < mineGambingInfo.Length; i++)
			{
				mineGambingInfo[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 mineGambingInfoCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref mineGambingInfoCnt);
			mineGambingInfo = new GambingMineInfo[mineGambingInfoCnt];
			for(int i = 0; i < mineGambingInfo.Length; i++)
			{
				mineGambingInfo[i] = new GambingMineInfo();
				mineGambingInfo[i].decode(buffer, ref pos_);
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