using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  结算
	/// </summary>
	[AdvancedInspector.Descriptor(" 结算", " 结算")]
	public class SceneMatchPkRaceEnd : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 506705;
		public UInt32 Sequence;
		/// <summary>
		///  PK类型，对应枚举(PkType)
		/// </summary>
		[AdvancedInspector.Descriptor(" PK类型，对应枚举(PkType)", " PK类型，对应枚举(PkType)")]
		public byte pkType;

		public byte result;

		public UInt32 oldPkValue;

		public UInt32 newPkValue;

		public UInt32 oldMatchScore;

		public UInt32 newMatchScore;
		/// <summary>
		///  初始决斗币数量
		/// </summary>
		[AdvancedInspector.Descriptor(" 初始决斗币数量", " 初始决斗币数量")]
		public UInt32 oldPkCoin;
		/// <summary>
		///  战斗获得的决斗币
		/// </summary>
		[AdvancedInspector.Descriptor(" 战斗获得的决斗币", " 战斗获得的决斗币")]
		public UInt32 addPkCoinFromRace;
		/// <summary>
		///  今日战斗获得的全部决斗币
		/// </summary>
		[AdvancedInspector.Descriptor(" 今日战斗获得的全部决斗币", " 今日战斗获得的全部决斗币")]
		public UInt32 totalPkCoinFromRace;
		/// <summary>
		///  是否在PVP活动期间
		/// </summary>
		[AdvancedInspector.Descriptor(" 是否在PVP活动期间", " 是否在PVP活动期间")]
		public byte isInPvPActivity;
		/// <summary>
		///  活动额外获得的决斗币
		/// </summary>
		[AdvancedInspector.Descriptor(" 活动额外获得的决斗币", " 活动额外获得的决斗币")]
		public UInt32 addPkCoinFromActivity;
		/// <summary>
		///  今日活动获得的全部决斗币
		/// </summary>
		[AdvancedInspector.Descriptor(" 今日活动获得的全部决斗币", " 今日活动获得的全部决斗币")]
		public UInt32 totalPkCoinFromActivity;
		/// <summary>
		///  原段位
		/// </summary>
		[AdvancedInspector.Descriptor(" 原段位", " 原段位")]
		public UInt32 oldSeasonLevel;
		/// <summary>
		///  现段位
		/// </summary>
		[AdvancedInspector.Descriptor(" 现段位", " 现段位")]
		public UInt32 newSeasonLevel;
		/// <summary>
		///  原星
		/// </summary>
		[AdvancedInspector.Descriptor(" 原星", " 原星")]
		public UInt32 oldSeasonStar;
		/// <summary>
		///  现星
		/// </summary>
		[AdvancedInspector.Descriptor(" 现星", " 现星")]
		public UInt32 newSeasonStar;
		/// <summary>
		///  原经验
		/// </summary>
		[AdvancedInspector.Descriptor(" 原经验", " 原经验")]
		public UInt32 oldSeasonExp;
		/// <summary>
		///  现经验
		/// </summary>
		[AdvancedInspector.Descriptor(" 现经验", " 现经验")]
		public UInt32 newSeasonExp;
		/// <summary>
		///  改变的经验
		/// </summary>
		[AdvancedInspector.Descriptor(" 改变的经验", " 改变的经验")]
		public Int32 changeSeasonExp;
		/// <summary>
		///  获得荣誉
		/// </summary>
		[AdvancedInspector.Descriptor(" 获得荣誉", " 获得荣誉")]
		public UInt32 getHonor;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, pkType);
			BaseDLL.encode_int8(buffer, ref pos_, result);
			BaseDLL.encode_uint32(buffer, ref pos_, oldPkValue);
			BaseDLL.encode_uint32(buffer, ref pos_, newPkValue);
			BaseDLL.encode_uint32(buffer, ref pos_, oldMatchScore);
			BaseDLL.encode_uint32(buffer, ref pos_, newMatchScore);
			BaseDLL.encode_uint32(buffer, ref pos_, oldPkCoin);
			BaseDLL.encode_uint32(buffer, ref pos_, addPkCoinFromRace);
			BaseDLL.encode_uint32(buffer, ref pos_, totalPkCoinFromRace);
			BaseDLL.encode_int8(buffer, ref pos_, isInPvPActivity);
			BaseDLL.encode_uint32(buffer, ref pos_, addPkCoinFromActivity);
			BaseDLL.encode_uint32(buffer, ref pos_, totalPkCoinFromActivity);
			BaseDLL.encode_uint32(buffer, ref pos_, oldSeasonLevel);
			BaseDLL.encode_uint32(buffer, ref pos_, newSeasonLevel);
			BaseDLL.encode_uint32(buffer, ref pos_, oldSeasonStar);
			BaseDLL.encode_uint32(buffer, ref pos_, newSeasonStar);
			BaseDLL.encode_uint32(buffer, ref pos_, oldSeasonExp);
			BaseDLL.encode_uint32(buffer, ref pos_, newSeasonExp);
			BaseDLL.encode_int32(buffer, ref pos_, changeSeasonExp);
			BaseDLL.encode_uint32(buffer, ref pos_, getHonor);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref pkType);
			BaseDLL.decode_int8(buffer, ref pos_, ref result);
			BaseDLL.decode_uint32(buffer, ref pos_, ref oldPkValue);
			BaseDLL.decode_uint32(buffer, ref pos_, ref newPkValue);
			BaseDLL.decode_uint32(buffer, ref pos_, ref oldMatchScore);
			BaseDLL.decode_uint32(buffer, ref pos_, ref newMatchScore);
			BaseDLL.decode_uint32(buffer, ref pos_, ref oldPkCoin);
			BaseDLL.decode_uint32(buffer, ref pos_, ref addPkCoinFromRace);
			BaseDLL.decode_uint32(buffer, ref pos_, ref totalPkCoinFromRace);
			BaseDLL.decode_int8(buffer, ref pos_, ref isInPvPActivity);
			BaseDLL.decode_uint32(buffer, ref pos_, ref addPkCoinFromActivity);
			BaseDLL.decode_uint32(buffer, ref pos_, ref totalPkCoinFromActivity);
			BaseDLL.decode_uint32(buffer, ref pos_, ref oldSeasonLevel);
			BaseDLL.decode_uint32(buffer, ref pos_, ref newSeasonLevel);
			BaseDLL.decode_uint32(buffer, ref pos_, ref oldSeasonStar);
			BaseDLL.decode_uint32(buffer, ref pos_, ref newSeasonStar);
			BaseDLL.decode_uint32(buffer, ref pos_, ref oldSeasonExp);
			BaseDLL.decode_uint32(buffer, ref pos_, ref newSeasonExp);
			BaseDLL.decode_int32(buffer, ref pos_, ref changeSeasonExp);
			BaseDLL.decode_uint32(buffer, ref pos_, ref getHonor);
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
