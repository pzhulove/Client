using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  删除npc
	/// </summary>
	[AdvancedInspector.Descriptor(" 删除npc", " 删除npc")]
	public class SceneNpcDel : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 500623;
		public UInt32 Sequence;
		/// <summary>
		///  npc guid列表
		/// </summary>
		[AdvancedInspector.Descriptor(" npc guid列表", " npc guid列表")]
		public UInt64[] guids = new UInt64[0];

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)guids.Length);
			for(int i = 0; i < guids.Length; i++)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, guids[i]);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 guidsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref guidsCnt);
			guids = new UInt64[guidsCnt];
			for(int i = 0; i < guids.Length; i++)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref guids[i]);
			}
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
