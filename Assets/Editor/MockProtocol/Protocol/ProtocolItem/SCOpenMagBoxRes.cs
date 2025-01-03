using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  server->client 
	/// </summary>
	[AdvancedInspector.Descriptor(" server->client ", " server->client ")]
	public class SCOpenMagBoxRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 500970;
		public UInt32 Sequence;

		public UInt32 retCode;

		public OpenJarResult[] rewards = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, retCode);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)rewards.Length);
			for(int i = 0; i < rewards.Length; i++)
			{
				rewards[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
			UInt16 rewardsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref rewardsCnt);
			rewards = new OpenJarResult[rewardsCnt];
			for(int i = 0; i < rewards.Length; i++)
			{
				rewards[i] = new OpenJarResult();
				rewards[i].decode(buffer, ref pos_);
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
