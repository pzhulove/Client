using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  scene->client 任务列表请求
	/// </summary>
	[AdvancedInspector.Descriptor(" scene->client 任务列表请求", " scene->client 任务列表请求")]
	public class SceneLostDungeonTaskListReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 510013;
		public UInt32 Sequence;
		/// <summary>
		///  难度
		/// </summary>
		[AdvancedInspector.Descriptor(" 难度", " 难度")]
		public UInt32 hardType;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, hardType);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref hardType);
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
