using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  返回申请列表
	/// </summary>
	[AdvancedInspector.Descriptor(" 返回申请列表", " 返回申请列表")]
	public class WorldTeamRequesterListRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601639;
		public UInt32 Sequence;

		public TeammemberBaseInfo[] requesters = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)requesters.Length);
			for(int i = 0; i < requesters.Length; i++)
			{
				requesters[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 requestersCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref requestersCnt);
			requesters = new TeammemberBaseInfo[requestersCnt];
			for(int i = 0; i < requesters.Length; i++)
			{
				requesters[i] = new TeammemberBaseInfo();
				requesters[i].decode(buffer, ref pos_);
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
