using System;
using System.Text;

namespace Mock.Protocol
{

	public class ShooterOddsRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 708304;
		public UInt32 Sequence;
		/// <summary>
		///  赔率列表
		/// </summary>
		[AdvancedInspector.Descriptor(" 赔率列表", " 赔率列表")]
		public OddsRateInfo[] oddsList = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)oddsList.Length);
			for(int i = 0; i < oddsList.Length; i++)
			{
				oddsList[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 oddsListCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref oddsListCnt);
			oddsList = new OddsRateInfo[oddsListCnt];
			for(int i = 0; i < oddsList.Length; i++)
			{
				oddsList[i] = new OddsRateInfo();
				oddsList[i].decode(buffer, ref pos_);
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
