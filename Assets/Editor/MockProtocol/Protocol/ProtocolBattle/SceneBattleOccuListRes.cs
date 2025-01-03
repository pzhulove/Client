using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  职业列表返回
	/// </summary>
	[AdvancedInspector.Descriptor(" 职业列表返回", " 职业列表返回")]
	public class SceneBattleOccuListRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 508948;
		public UInt32 Sequence;

		public UInt32[] occuList = new UInt32[0];

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)occuList.Length);
			for(int i = 0; i < occuList.Length; i++)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, occuList[i]);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 occuListCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref occuListCnt);
			occuList = new UInt32[occuListCnt];
			for(int i = 0; i < occuList.Length; i++)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref occuList[i]);
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
