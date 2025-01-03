using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  录像信息
	/// </summary>
	[AdvancedInspector.Descriptor(" 录像信息", " 录像信息")]
	public class ReplayInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  版本号
		/// </summary>
		[AdvancedInspector.Descriptor(" 版本号", " 版本号")]
		public UInt32 version;
		/// <summary>
		///  比赛ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 比赛ID", " 比赛ID")]
		public UInt64 raceId;
		/// <summary>
		///  pk类型（对应枚举PkType）
		/// </summary>
		[AdvancedInspector.Descriptor(" pk类型（对应枚举PkType）", " pk类型（对应枚举PkType）")]
		public byte type;
		/// <summary>
		///  评价(对应枚举ReplayEvaluation)
		/// </summary>
		[AdvancedInspector.Descriptor(" 评价(对应枚举ReplayEvaluation)", " 评价(对应枚举ReplayEvaluation)")]
		public byte evaluation;
		/// <summary>
		///  记录时间
		/// </summary>
		[AdvancedInspector.Descriptor(" 记录时间", " 记录时间")]
		public UInt32 recordTime;
		/// <summary>
		///  战斗结果（第一个人的结果,对应枚举PkRaceResult）
		/// </summary>
		[AdvancedInspector.Descriptor(" 战斗结果（第一个人的结果,对应枚举PkRaceResult）", " 战斗结果（第一个人的结果,对应枚举PkRaceResult）")]
		public byte result;
		/// <summary>
		///  战斗信息
		/// </summary>
		[AdvancedInspector.Descriptor(" 战斗信息", " 战斗信息")]
		public ReplayFighterInfo[] fighters = null;
		/// <summary>
		///  观看次数
		/// </summary>
		[AdvancedInspector.Descriptor(" 观看次数", " 观看次数")]
		public UInt32 viewNum;
		/// <summary>
		///  分数
		/// </summary>
		[AdvancedInspector.Descriptor(" 分数", " 分数")]
		public UInt32 score;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, version);
			BaseDLL.encode_uint64(buffer, ref pos_, raceId);
			BaseDLL.encode_int8(buffer, ref pos_, type);
			BaseDLL.encode_int8(buffer, ref pos_, evaluation);
			BaseDLL.encode_uint32(buffer, ref pos_, recordTime);
			BaseDLL.encode_int8(buffer, ref pos_, result);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)fighters.Length);
			for(int i = 0; i < fighters.Length; i++)
			{
				fighters[i].encode(buffer, ref pos_);
			}
			BaseDLL.encode_uint32(buffer, ref pos_, viewNum);
			BaseDLL.encode_uint32(buffer, ref pos_, score);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref version);
			BaseDLL.decode_uint64(buffer, ref pos_, ref raceId);
			BaseDLL.decode_int8(buffer, ref pos_, ref type);
			BaseDLL.decode_int8(buffer, ref pos_, ref evaluation);
			BaseDLL.decode_uint32(buffer, ref pos_, ref recordTime);
			BaseDLL.decode_int8(buffer, ref pos_, ref result);
			UInt16 fightersCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref fightersCnt);
			fighters = new ReplayFighterInfo[fightersCnt];
			for(int i = 0; i < fighters.Length; i++)
			{
				fighters[i] = new ReplayFighterInfo();
				fighters[i].decode(buffer, ref pos_);
			}
			BaseDLL.decode_uint32(buffer, ref pos_, ref viewNum);
			BaseDLL.decode_uint32(buffer, ref pos_, ref score);
		}


		#endregion

	}

}
