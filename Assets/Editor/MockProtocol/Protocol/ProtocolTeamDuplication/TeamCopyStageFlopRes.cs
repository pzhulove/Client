using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  翻牌
	/// </summary>
	[AdvancedInspector.Descriptor(" 翻牌", " 翻牌")]
	public class TeamCopyStageFlopRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 1100036;
		public UInt32 Sequence;
		/// <summary>
		///  阶段id
		/// </summary>
		[AdvancedInspector.Descriptor(" 阶段id", " 阶段id")]
		public UInt32 stageId;
		/// <summary>
		///  翻牌列表
		/// </summary>
		[AdvancedInspector.Descriptor(" 翻牌列表", " 翻牌列表")]
		public TeamCopyFlop[] flopList = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, stageId);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)flopList.Length);
			for(int i = 0; i < flopList.Length; i++)
			{
				flopList[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref stageId);
			UInt16 flopListCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref flopListCnt);
			flopList = new TeamCopyFlop[flopListCnt];
			for(int i = 0; i < flopList.Length; i++)
			{
				flopList[i] = new TeamCopyFlop();
				flopList[i].decode(buffer, ref pos_);
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
