using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  请求活动怪物信息
	/// </summary>
	[AdvancedInspector.Descriptor(" 请求活动怪物信息", " 请求活动怪物信息")]
	public class WorldActivityMonsterReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 607404;
		public UInt32 Sequence;

		public UInt32 activityId;

		public UInt32[] ids = new UInt32[0];

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, activityId);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)ids.Length);
			for(int i = 0; i < ids.Length; i++)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, ids[i]);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref activityId);
			UInt16 idsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref idsCnt);
			ids = new UInt32[idsCnt];
			for(int i = 0; i < ids.Length; i++)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref ids[i]);
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
