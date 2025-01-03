using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// Client ->Scene 请求一组的比赛记录
	/// </summary>
	[AdvancedInspector.Descriptor("Client ->Scene 请求一组的比赛记录", "Client ->Scene 请求一组的比赛记录")]
	public class SceneChampionGroupRecordReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 509825;
		public UInt32 Sequence;
		/// <summary>
		/// 组id
		/// </summary>
		[AdvancedInspector.Descriptor("组id", "组id")]
		public UInt32 groupID;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, groupID);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref groupID);
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
