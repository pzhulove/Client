using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// Scene->Client 请求观战返回
	/// </summary>
	[AdvancedInspector.Descriptor("Scene->Client 请求观战返回", "Scene->Client 请求观战返回")]
	public class SceneChampionObserveRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 509816;
		public UInt32 Sequence;

		public UInt64 roleID;

		public UInt64 raceID;

		public SockAddr addr = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, roleID);
			BaseDLL.encode_uint64(buffer, ref pos_, raceID);
			addr.encode(buffer, ref pos_);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref roleID);
			BaseDLL.decode_uint64(buffer, ref pos_, ref raceID);
			addr.decode(buffer, ref pos_);
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
