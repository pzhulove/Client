using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// Client ->Scene 请求冠军赛状态
	/// </summary>
	[AdvancedInspector.Descriptor("Client ->Scene 请求冠军赛状态", "Client ->Scene 请求冠军赛状态")]
	public class SceneChampionStatusReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 509817;
		public UInt32 Sequence;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
		}

		public void decode(byte[] buffer, ref int pos_)
		{
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
