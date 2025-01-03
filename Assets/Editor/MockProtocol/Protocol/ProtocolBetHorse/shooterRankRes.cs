using System;
using System.Text;

namespace Mock.Protocol
{

	public class shooterRankRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 708315;
		public UInt32 Sequence;
		/// <summary>
		///  排行列表
		/// </summary>
		[AdvancedInspector.Descriptor(" 排行列表", " 排行列表")]
		public shooterRankInfo[] shooterRankList = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)shooterRankList.Length);
			for(int i = 0; i < shooterRankList.Length; i++)
			{
				shooterRankList[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 shooterRankListCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref shooterRankListCnt);
			shooterRankList = new shooterRankInfo[shooterRankListCnt];
			for(int i = 0; i < shooterRankList.Length; i++)
			{
				shooterRankList[i] = new shooterRankInfo();
				shooterRankList[i].decode(buffer, ref pos_);
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
