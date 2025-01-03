using System;
using System.Text;

namespace Mock.Protocol
{

	public class RacePlayerInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  服务器ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 服务器ID", " 服务器ID")]
		public UInt32 zoneId;
		/// <summary>
		///  服务器名
		/// </summary>
		[AdvancedInspector.Descriptor(" 服务器名", " 服务器名")]
		public string serverName;
		/// <summary>
		///  ai难度，0代表无效值，说明不是机器人
		/// </summary>
		[AdvancedInspector.Descriptor(" ai难度，0代表无效值，说明不是机器人", " ai难度，0代表无效值，说明不是机器人")]
		public byte robotAIType;
		/// <summary>
		///  机器人难度（0代表满血）
		/// </summary>
		[AdvancedInspector.Descriptor(" 机器人难度（0代表满血）", " 机器人难度（0代表满血）")]
		public byte robotHard;

		public UInt64 roleId;

		public UInt32 accid;

		public string name;

		public string guildName;

		public byte occupation;

		public UInt16 level;

		public UInt32 pkValue;

		public UInt32 matchScore;

		public byte seat;

		public UInt32 remainHp;

		public UInt32 remainMp;

		public UInt32 seasonLevel;

		public UInt32 seasonStar;

		public byte seasonAttr;

		public byte monthcard;

		public RaceSkillInfo[] skills = null;

		public RaceEquip[] equips = null;

		public RaceItem[] raceItems = null;

		public RaceBuffInfo[] buffs = null;

		public RaceWarpStone[] warpStones = null;

		public RaceRetinue[] retinues = null;

		public RacePet[] pets = null;

		public UInt32[] potionPos = new UInt32[0];

		public RaceEquip[] secondWeapons = null;

		public PlayerAvatar avatar = null;
		/// <summary>
		/// 玩家标签信息
		/// </summary>
		[AdvancedInspector.Descriptor("玩家标签信息", "玩家标签信息")]
		public PlayerLabelInfo playerLabelInfo = null;
		/// <summary>
		/// 装备方案
		/// </summary>
		[AdvancedInspector.Descriptor("装备方案", "装备方案")]
		public RaceEquipScheme[] equipScheme = null;
		/// <summary>
		/// 当前穿戴装备,时装id
		/// </summary>
		[AdvancedInspector.Descriptor("当前穿戴装备,时装id", "当前穿戴装备,时装id")]
		public UInt64[] wearingEqIds = new UInt64[0];
		/// <summary>
		/// 地牢神器
		/// </summary>
		[AdvancedInspector.Descriptor("地牢神器", "地牢神器")]
		public RaceItem[] lostDungArtifacts = null;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, zoneId);
			byte[] serverNameBytes = StringHelper.StringToUTF8Bytes(serverName);
			BaseDLL.encode_string(buffer, ref pos_, serverNameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_int8(buffer, ref pos_, robotAIType);
			BaseDLL.encode_int8(buffer, ref pos_, robotHard);
			BaseDLL.encode_uint64(buffer, ref pos_, roleId);
			BaseDLL.encode_uint32(buffer, ref pos_, accid);
			byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
			BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
			byte[] guildNameBytes = StringHelper.StringToUTF8Bytes(guildName);
			BaseDLL.encode_string(buffer, ref pos_, guildNameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_int8(buffer, ref pos_, occupation);
			BaseDLL.encode_uint16(buffer, ref pos_, level);
			BaseDLL.encode_uint32(buffer, ref pos_, pkValue);
			BaseDLL.encode_uint32(buffer, ref pos_, matchScore);
			BaseDLL.encode_int8(buffer, ref pos_, seat);
			BaseDLL.encode_uint32(buffer, ref pos_, remainHp);
			BaseDLL.encode_uint32(buffer, ref pos_, remainMp);
			BaseDLL.encode_uint32(buffer, ref pos_, seasonLevel);
			BaseDLL.encode_uint32(buffer, ref pos_, seasonStar);
			BaseDLL.encode_int8(buffer, ref pos_, seasonAttr);
			BaseDLL.encode_int8(buffer, ref pos_, monthcard);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)skills.Length);
			for(int i = 0; i < skills.Length; i++)
			{
				skills[i].encode(buffer, ref pos_);
			}
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)equips.Length);
			for(int i = 0; i < equips.Length; i++)
			{
				equips[i].encode(buffer, ref pos_);
			}
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)raceItems.Length);
			for(int i = 0; i < raceItems.Length; i++)
			{
				raceItems[i].encode(buffer, ref pos_);
			}
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)buffs.Length);
			for(int i = 0; i < buffs.Length; i++)
			{
				buffs[i].encode(buffer, ref pos_);
			}
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)warpStones.Length);
			for(int i = 0; i < warpStones.Length; i++)
			{
				warpStones[i].encode(buffer, ref pos_);
			}
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)retinues.Length);
			for(int i = 0; i < retinues.Length; i++)
			{
				retinues[i].encode(buffer, ref pos_);
			}
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)pets.Length);
			for(int i = 0; i < pets.Length; i++)
			{
				pets[i].encode(buffer, ref pos_);
			}
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)potionPos.Length);
			for(int i = 0; i < potionPos.Length; i++)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, potionPos[i]);
			}
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)secondWeapons.Length);
			for(int i = 0; i < secondWeapons.Length; i++)
			{
				secondWeapons[i].encode(buffer, ref pos_);
			}
			avatar.encode(buffer, ref pos_);
			playerLabelInfo.encode(buffer, ref pos_);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)equipScheme.Length);
			for(int i = 0; i < equipScheme.Length; i++)
			{
				equipScheme[i].encode(buffer, ref pos_);
			}
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)wearingEqIds.Length);
			for(int i = 0; i < wearingEqIds.Length; i++)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, wearingEqIds[i]);
			}
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)lostDungArtifacts.Length);
			for(int i = 0; i < lostDungArtifacts.Length; i++)
			{
				lostDungArtifacts[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref zoneId);
			UInt16 serverNameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref serverNameLen);
			byte[] serverNameBytes = new byte[serverNameLen];
			for(int i = 0; i < serverNameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref serverNameBytes[i]);
			}
			serverName = StringHelper.BytesToString(serverNameBytes);
			BaseDLL.decode_int8(buffer, ref pos_, ref robotAIType);
			BaseDLL.decode_int8(buffer, ref pos_, ref robotHard);
			BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref accid);
			UInt16 nameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
			byte[] nameBytes = new byte[nameLen];
			for(int i = 0; i < nameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
			}
			name = StringHelper.BytesToString(nameBytes);
			UInt16 guildNameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref guildNameLen);
			byte[] guildNameBytes = new byte[guildNameLen];
			for(int i = 0; i < guildNameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref guildNameBytes[i]);
			}
			guildName = StringHelper.BytesToString(guildNameBytes);
			BaseDLL.decode_int8(buffer, ref pos_, ref occupation);
			BaseDLL.decode_uint16(buffer, ref pos_, ref level);
			BaseDLL.decode_uint32(buffer, ref pos_, ref pkValue);
			BaseDLL.decode_uint32(buffer, ref pos_, ref matchScore);
			BaseDLL.decode_int8(buffer, ref pos_, ref seat);
			BaseDLL.decode_uint32(buffer, ref pos_, ref remainHp);
			BaseDLL.decode_uint32(buffer, ref pos_, ref remainMp);
			BaseDLL.decode_uint32(buffer, ref pos_, ref seasonLevel);
			BaseDLL.decode_uint32(buffer, ref pos_, ref seasonStar);
			BaseDLL.decode_int8(buffer, ref pos_, ref seasonAttr);
			BaseDLL.decode_int8(buffer, ref pos_, ref monthcard);
			UInt16 skillsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref skillsCnt);
			skills = new RaceSkillInfo[skillsCnt];
			for(int i = 0; i < skills.Length; i++)
			{
				skills[i] = new RaceSkillInfo();
				skills[i].decode(buffer, ref pos_);
			}
			UInt16 equipsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref equipsCnt);
			equips = new RaceEquip[equipsCnt];
			for(int i = 0; i < equips.Length; i++)
			{
				equips[i] = new RaceEquip();
				equips[i].decode(buffer, ref pos_);
			}
			UInt16 raceItemsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref raceItemsCnt);
			raceItems = new RaceItem[raceItemsCnt];
			for(int i = 0; i < raceItems.Length; i++)
			{
				raceItems[i] = new RaceItem();
				raceItems[i].decode(buffer, ref pos_);
			}
			UInt16 buffsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref buffsCnt);
			buffs = new RaceBuffInfo[buffsCnt];
			for(int i = 0; i < buffs.Length; i++)
			{
				buffs[i] = new RaceBuffInfo();
				buffs[i].decode(buffer, ref pos_);
			}
			UInt16 warpStonesCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref warpStonesCnt);
			warpStones = new RaceWarpStone[warpStonesCnt];
			for(int i = 0; i < warpStones.Length; i++)
			{
				warpStones[i] = new RaceWarpStone();
				warpStones[i].decode(buffer, ref pos_);
			}
			UInt16 retinuesCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref retinuesCnt);
			retinues = new RaceRetinue[retinuesCnt];
			for(int i = 0; i < retinues.Length; i++)
			{
				retinues[i] = new RaceRetinue();
				retinues[i].decode(buffer, ref pos_);
			}
			UInt16 petsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref petsCnt);
			pets = new RacePet[petsCnt];
			for(int i = 0; i < pets.Length; i++)
			{
				pets[i] = new RacePet();
				pets[i].decode(buffer, ref pos_);
			}
			UInt16 potionPosCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref potionPosCnt);
			potionPos = new UInt32[potionPosCnt];
			for(int i = 0; i < potionPos.Length; i++)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref potionPos[i]);
			}
			UInt16 secondWeaponsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref secondWeaponsCnt);
			secondWeapons = new RaceEquip[secondWeaponsCnt];
			for(int i = 0; i < secondWeapons.Length; i++)
			{
				secondWeapons[i] = new RaceEquip();
				secondWeapons[i].decode(buffer, ref pos_);
			}
			avatar.decode(buffer, ref pos_);
			playerLabelInfo.decode(buffer, ref pos_);
			UInt16 equipSchemeCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref equipSchemeCnt);
			equipScheme = new RaceEquipScheme[equipSchemeCnt];
			for(int i = 0; i < equipScheme.Length; i++)
			{
				equipScheme[i] = new RaceEquipScheme();
				equipScheme[i].decode(buffer, ref pos_);
			}
			UInt16 wearingEqIdsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref wearingEqIdsCnt);
			wearingEqIds = new UInt64[wearingEqIdsCnt];
			for(int i = 0; i < wearingEqIds.Length; i++)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref wearingEqIds[i]);
			}
			UInt16 lostDungArtifactsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref lostDungArtifactsCnt);
			lostDungArtifacts = new RaceItem[lostDungArtifactsCnt];
			for(int i = 0; i < lostDungArtifacts.Length; i++)
			{
				lostDungArtifacts[i] = new RaceItem();
				lostDungArtifacts[i].decode(buffer, ref pos_);
			}
		}


		#endregion

	}

}
