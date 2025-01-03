using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 查询推荐关系列表
	/// </summary>
	[AdvancedInspector.Descriptor("查询推荐关系列表", "查询推荐关系列表")]
	public class WorldRelationFindPlayersRet : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601710;
		public UInt32 Sequence;

		public byte type;

		public QuickFriendInfo[] friendList = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, type);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)friendList.Length);
			for(int i = 0; i < friendList.Length; i++)
			{
				friendList[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref type);
			UInt16 friendListCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref friendListCnt);
			friendList = new QuickFriendInfo[friendListCnt];
			for(int i = 0; i < friendList.Length; i++)
			{
				friendList[i] = new QuickFriendInfo();
				friendList[i].decode(buffer, ref pos_);
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
