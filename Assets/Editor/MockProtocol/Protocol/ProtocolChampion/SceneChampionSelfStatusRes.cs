using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// Scene->Client 请求自己阶段返回
	/// </summary>
	[AdvancedInspector.Descriptor("Scene->Client 请求自己阶段返回", "Scene->Client 请求自己阶段返回")]
	public class SceneChampionSelfStatusRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 509820;
		public UInt32 Sequence;
		/// <summary>
		/// ChampionStatus
		/// </summary>
		[AdvancedInspector.Descriptor("ChampionStatus", "ChampionStatus")]
		public UInt32 status;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, status);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref status);
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
