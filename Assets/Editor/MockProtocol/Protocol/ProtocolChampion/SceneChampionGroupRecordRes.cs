using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// Scene->Client 请求一组的比赛记录返回
	/// </summary>
	[AdvancedInspector.Descriptor("Scene->Client 请求一组的比赛记录返回", "Scene->Client 请求一组的比赛记录返回")]
	public class SceneChampionGroupRecordRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 509826;
		public UInt32 Sequence;
		/// <summary>
		/// 组id
		/// </summary>
		[AdvancedInspector.Descriptor("组id", "组id")]
		public UInt32 groupID;
		/// <summary>
		/// 比赛记录
		/// </summary>
		[AdvancedInspector.Descriptor("比赛记录", "比赛记录")]
		public ChampionBattleRecord[] records = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, groupID);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)records.Length);
			for(int i = 0; i < records.Length; i++)
			{
				records[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref groupID);
			UInt16 recordsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref recordsCnt);
			records = new ChampionBattleRecord[recordsCnt];
			for(int i = 0; i < records.Length; i++)
			{
				records[i] = new ChampionBattleRecord();
				records[i].decode(buffer, ref pos_);
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
