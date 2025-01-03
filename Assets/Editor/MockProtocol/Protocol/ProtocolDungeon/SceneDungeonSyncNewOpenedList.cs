using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 同步新开放的地下城列表
	/// </summary>
	[AdvancedInspector.Descriptor("同步新开放的地下城列表", "同步新开放的地下城列表")]
	public class SceneDungeonSyncNewOpenedList : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 506822;
		public UInt32 Sequence;
		/// <summary>
		/// 新开放的地下城列表
		/// </summary>
		[AdvancedInspector.Descriptor("新开放的地下城列表", "新开放的地下城列表")]
		public DungeonOpenInfo[] newOpenedList = null;
		/// <summary>
		/// 新关闭掉的地下城列表
		/// </summary>
		[AdvancedInspector.Descriptor("新关闭掉的地下城列表", "新关闭掉的地下城列表")]
		public UInt32[] newClosedList = new UInt32[0];

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)newOpenedList.Length);
			for(int i = 0; i < newOpenedList.Length; i++)
			{
				newOpenedList[i].encode(buffer, ref pos_);
			}
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)newClosedList.Length);
			for(int i = 0; i < newClosedList.Length; i++)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, newClosedList[i]);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 newOpenedListCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref newOpenedListCnt);
			newOpenedList = new DungeonOpenInfo[newOpenedListCnt];
			for(int i = 0; i < newOpenedList.Length; i++)
			{
				newOpenedList[i] = new DungeonOpenInfo();
				newOpenedList[i].decode(buffer, ref pos_);
			}
			UInt16 newClosedListCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref newClosedListCnt);
			newClosedList = new UInt32[newClosedListCnt];
			for(int i = 0; i < newClosedList.Length; i++)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref newClosedList[i]);
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
