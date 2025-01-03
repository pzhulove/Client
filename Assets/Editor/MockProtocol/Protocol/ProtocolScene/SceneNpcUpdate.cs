using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  修改npc状态
	/// </summary>
	[AdvancedInspector.Descriptor(" 修改npc状态", " 修改npc状态")]
	public class SceneNpcUpdate : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 500624;
		public UInt32 Sequence;
		/// <summary>
		///  npc最新信息
		/// </summary>
		[AdvancedInspector.Descriptor(" npc最新信息", " npc最新信息")]
		public SceneNpcInfo data = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			data.encode(buffer, ref pos_);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			data.decode(buffer, ref pos_);
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
