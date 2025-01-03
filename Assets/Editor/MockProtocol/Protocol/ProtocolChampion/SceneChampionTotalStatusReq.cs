using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// Client ->Scene 请求冠军赛和自己阶段
	/// </summary>
	[AdvancedInspector.Descriptor("Client ->Scene 请求冠军赛和自己阶段", "Client ->Scene 请求冠军赛和自己阶段")]
	public class SceneChampionTotalStatusReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 509827;
		public UInt32 Sequence;

		public UInt32 accid;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, accid);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref accid);
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
