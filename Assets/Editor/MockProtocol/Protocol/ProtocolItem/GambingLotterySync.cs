using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// world->client 
	/// </summary>
	[AdvancedInspector.Descriptor("world->client ", "world->client ")]
	public class GambingLotterySync : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 707909;
		public UInt32 Sequence;
		/// <summary>
		///  获得者夺宝数据
		/// </summary>
		[AdvancedInspector.Descriptor(" 获得者夺宝数据", " 获得者夺宝数据")]
		public GambingParticipantInfo gainersGambingInfo = null;
		/// <summary>
		///  参与者夺宝数据
		/// </summary>
		[AdvancedInspector.Descriptor(" 参与者夺宝数据", " 参与者夺宝数据")]
		public GambingParticipantInfo[] participantsGambingInfo = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			gainersGambingInfo.encode(buffer, ref pos_);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)participantsGambingInfo.Length);
			for(int i = 0; i < participantsGambingInfo.Length; i++)
			{
				participantsGambingInfo[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			gainersGambingInfo.decode(buffer, ref pos_);
			UInt16 participantsGambingInfoCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref participantsGambingInfoCnt);
			participantsGambingInfo = new GambingParticipantInfo[participantsGambingInfoCnt];
			for(int i = 0; i < participantsGambingInfo.Length; i++)
			{
				participantsGambingInfo[i] = new GambingParticipantInfo();
				participantsGambingInfo[i].decode(buffer, ref pos_);
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
