using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// Scene->Client 请求冠军赛和自己阶段返回
	/// </summary>
	[AdvancedInspector.Descriptor("Scene->Client 请求冠军赛和自己阶段返回", "Scene->Client 请求冠军赛和自己阶段返回")]
	public class SceneChampionTotalStatusRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 509828;
		public UInt32 Sequence;
		/// <summary>
		/// ChampionStatus
		/// </summary>
		[AdvancedInspector.Descriptor("ChampionStatus", "ChampionStatus")]
		public ChampionStatusInfo status = null;

		public UInt32 slefStatus;

		public UInt64 roleID;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			status.encode(buffer, ref pos_);
			BaseDLL.encode_uint32(buffer, ref pos_, slefStatus);
			BaseDLL.encode_uint64(buffer, ref pos_, roleID);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			status.decode(buffer, ref pos_);
			BaseDLL.decode_uint32(buffer, ref pos_, ref slefStatus);
			BaseDLL.decode_uint64(buffer, ref pos_, ref roleID);
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
