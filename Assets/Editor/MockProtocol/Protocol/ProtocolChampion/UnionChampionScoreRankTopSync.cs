using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 积分排行前5名(用于显示小排行)
	/// </summary>
	[AdvancedInspector.Descriptor("积分排行前5名(用于显示小排行)", "积分排行前5名(用于显示小排行)")]
	public class UnionChampionScoreRankTopSync : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 2709811;
		public UInt32 Sequence;

		public RankRole[] rankList = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)rankList.Length);
			for(int i = 0; i < rankList.Length; i++)
			{
				rankList[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 rankListCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref rankListCnt);
			rankList = new RankRole[rankListCnt];
			for(int i = 0; i < rankList.Length; i++)
			{
				rankList[i] = new RankRole();
				rankList[i].decode(buffer, ref pos_);
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