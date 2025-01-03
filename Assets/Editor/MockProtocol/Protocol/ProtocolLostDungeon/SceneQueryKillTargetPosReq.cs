using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// client->scene  查询击杀目标坐标请求
	/// </summary>
	[AdvancedInspector.Descriptor("client->scene  查询击杀目标坐标请求", "client->scene  查询击杀目标坐标请求")]
	public class SceneQueryKillTargetPosReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 510033;
		public UInt32 Sequence;

		public UInt64[] playerIds = new UInt64[0];

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)playerIds.Length);
			for(int i = 0; i < playerIds.Length; i++)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, playerIds[i]);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 playerIdsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref playerIdsCnt);
			playerIds = new UInt64[playerIdsCnt];
			for(int i = 0; i < playerIds.Length; i++)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref playerIds[i]);
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
