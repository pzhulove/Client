using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  请求开始组队匹配
	/// </summary>
	[AdvancedInspector.Descriptor(" 请求开始组队匹配", " 请求开始组队匹配")]
	public class SceneTeamMatchStartReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501604;
		public UInt32 Sequence;
		/// <summary>
		///  目标地下城ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 目标地下城ID", " 目标地下城ID")]
		public UInt32 dungeonId;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, dungeonId);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref dungeonId);
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
