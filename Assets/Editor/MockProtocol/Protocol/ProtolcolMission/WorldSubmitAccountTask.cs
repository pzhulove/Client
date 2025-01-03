using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// client->world 提交一个账号任务
	/// </summary>
	[AdvancedInspector.Descriptor("client->world 提交一个账号任务", "client->world 提交一个账号任务")]
	public class WorldSubmitAccountTask : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601103;
		public UInt32 Sequence;
		/// <summary>
		///  任务id
		/// </summary>
		[AdvancedInspector.Descriptor(" 任务id", " 任务id")]
		public UInt32 taskId;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, taskId);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref taskId);
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
