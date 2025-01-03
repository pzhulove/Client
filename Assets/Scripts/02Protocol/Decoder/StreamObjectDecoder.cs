using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using System.Reflection;

namespace Protocol
{
    public class SPropertyAttribute : Attribute
    {
        public int id;
        public SPropertyAttribute(int id)
        {
            this.id = id;
        }
    }

    public class StreamObject
    {
        public List<int> dirtyFields = new List<int>();
        public virtual bool GetStructProperty(FieldInfo field, byte[] buffer, ref int pos, int length)
        {
            return false;
        }
    }

    public class StreamObjectDecoder<T> where T : StreamObject
    {
        protected static Dictionary<int, FieldInfo> fieldDict;

        protected static void InitFieldDict()
        {
            if (fieldDict != null)
            {
                return;
            }

            System.Type t = typeof(T);
            FieldInfo[] fieldInfo = t.GetFields(BindingFlags.Instance | BindingFlags.Public);

            Dictionary<int, FieldInfo> fdics = new Dictionary<int, FieldInfo>();

            for (int i = 0; i < fieldInfo.Length; ++i)
            {
                var current = fieldInfo[i];

                object[] ps = current.GetCustomAttributes(typeof(SPropertyAttribute), false);

                if (ps.Length > 0)
                {
                    SPropertyAttribute attr = ps[0] as SPropertyAttribute;
                    fdics.Add(attr.id, current);
                }
            }

            fieldDict = fdics;
        }

        public static bool DecodePropertys(ref T streamObj, byte[] buffer, ref int pos, int length)
        {
            streamObj.dirtyFields.Clear();

            while (DecodeProperty(ref streamObj, buffer, ref pos, length)) ;

            return true;
        }
        private static bool DecodeProperty(ref T streamObj, byte[] buffer, ref int pos, int length)
        {
            InitFieldDict();

            if (pos >= length)
            {
                return false;
            }

            byte fieldId = 0;
            BaseDLL.decode_int8(buffer, ref pos, ref fieldId);
            if (fieldId == 0)
            {
                return false;
            }

            FieldInfo field;
            if (false == fieldDict.TryGetValue(fieldId, out field))
            {
                //Error!!!
#if MG_TEST || UNITY_EDITOR
                Logger.LogErrorFormat("unknown field attr:{0}", fieldId);
#else
                Logger.LogWarningFormat("unknown field attr:{0}", fieldId);
#endif
                return false;
            }

            object value = new object();
            if (!GetPropertyValue(ref value, field, buffer, ref pos, length))
            {
                if(!streamObj.GetStructProperty(field, buffer, ref pos, length))
                {
                    Logger.LogErrorFormat("decode field attr:{0} failed", fieldId);
                    return false;
                }
            }
            else
            {
                field.SetValue(streamObj, value);
            }

            streamObj.dirtyFields.Add(fieldId);
            return true;
        }

        static bool GetPropertyValue(ref object value, FieldInfo field, byte[] buffer, ref int pos, int length)
        {
			if (field.FieldType == typeof(byte))
			{
				byte val = 0;
				BaseDLL.decode_int8(buffer, ref pos, ref val);
				value = val;
				return true;
			}
			else if (field.FieldType == typeof(UInt16))
			{
				UInt16 val = 0;
				BaseDLL.decode_uint16(buffer, ref pos, ref val);
				value = val;
				return true;
			}
			else if (field.FieldType == typeof(UInt32))
			{
				UInt32 val = 0;
				BaseDLL.decode_uint32(buffer, ref pos, ref val);
				value = val;
				return true;
			}
            else if (field.FieldType == typeof(Int32))
            {
                Int32 val = 0;
                BaseDLL.decode_int32(buffer, ref pos, ref val);
                value = val;
                return true;
            }
			else if (field.FieldType == typeof(UInt64))
			{
				UInt64 val = 0;
				BaseDLL.decode_uint64(buffer, ref pos, ref val);
				value = val;
				return true;
			}
			else if (field.FieldType == typeof(string))
			{
				string val;

				UInt16 strLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos, ref strLen);
				byte[] strBytes = new byte[strLen];
				for (int i = 0; i < strLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos, ref strBytes[i]);
				}
				val = Encoding.UTF8.GetString(strBytes);

				value = val;
				return true;
			}
			else if (field.FieldType == typeof(ObjectPos))
			{
				ObjectPos val = new ObjectPos();
				BaseDLL.decode_uint16(buffer, ref pos, ref val.x);
				BaseDLL.decode_uint16(buffer, ref pos, ref val.y);

				value = val;
				return true;
			}
			else if (field.FieldType == typeof(QQVipInfo))
			{
				QQVipInfo val = new QQVipInfo();
				BaseDLL.decode_int8(buffer, ref pos, ref val.flag);
				BaseDLL.decode_int8(buffer, ref pos, ref val.level);
				BaseDLL.decode_int8(buffer, ref pos, ref val.lv3366);

				value = val;
				return true;
			}
			else if (field.FieldType == typeof(ScenePosition))
			{
				ScenePosition val = new ScenePosition();
				val.decode(buffer, ref pos);
				value = val;
				return true;
			}
			else if (field.FieldType == typeof(SceneDir))
			{
				SceneDir val = new SceneDir();
				val.decode(buffer, ref pos);
				value = val;
				return true;
			}
			else if (field.FieldType == typeof(List<Skill>))
			{
				List<Skill> skills = new List<Skill>();
				while (true)
				{
					Skill val = new Skill();
					BaseDLL.decode_uint16(buffer, ref pos, ref val.id);
					if (val.id == 0) break;
					BaseDLL.decode_int8(buffer, ref pos, ref val.level);
					skills.Add(val);
				}
				value = skills;
				return true;
			}
			else if (field.FieldType == typeof(List<Battle.DungeonBuff>))
			{
				List<Battle.DungeonBuff> bufflist = new List<Battle.DungeonBuff>();

				while (true)
				{
					Buff buff = new Buff();
					BaseDLL.decode_uint32(buffer, ref pos, ref buff.id);
					if (buff.id == 0) break;

					BaseDLL.decode_uint64(buffer, ref pos, ref buff.uid);
					BaseDLL.decode_uint32(buffer, ref pos, ref buff.overlay);
					BaseDLL.decode_uint32(buffer, ref pos, ref buff.duration);

					bufflist.Add(new Battle.DungeonBuff()
					{
						uid = buff.uid,
						id = (int)buff.id,
						overlay = (int)buff.overlay,
						duration = (float)buff.duration,
						lefttime = (float)buff.duration
					});
				}

				value = bufflist;
				return true;
			}
			else if (field.FieldType == typeof(SkillBars))
			{
				SkillBars skillBars = new SkillBars();
				skillBars.decode(buffer, ref pos);
				value = skillBars;
				return true;
			}
			else if (field.FieldType == typeof(List<ItemCD>))
			{
				List<ItemCD> itemCd = new List<ItemCD>();
				while (true)
				{
					ItemCD val = new ItemCD();
					BaseDLL.decode_int8(buffer, ref pos, ref val.groupid);
					if (val.groupid == 0) break;
					BaseDLL.decode_uint32(buffer, ref pos, ref val.endtime);
					BaseDLL.decode_uint32(buffer, ref pos, ref val.maxtime);
					itemCd.Add(val);
				}
				value = itemCd;
				return true;
			}
			else if (field.FieldType == typeof(List<WarpStoneInfo>))
			{
				List<WarpStoneInfo> warpStoneInfos = new List<WarpStoneInfo>();
				while (true)
				{
					//id(UInt32) + level(UInt8) + energy(UInt32)... 0(UInt32)
					WarpStoneInfo val = new WarpStoneInfo();
					BaseDLL.decode_uint32(buffer, ref pos, ref val.id);
					if (val.id == 0) break;
					BaseDLL.decode_int8(buffer, ref pos, ref val.level);
					BaseDLL.decode_uint32(buffer, ref pos, ref val.energy);
					warpStoneInfos.Add(val);
				}
				value = warpStoneInfos;
				return true;
			}
			else if (field.FieldType == typeof(FuncMaskProperty))
			{
				FuncMaskProperty funcMask = new FuncMaskProperty();
				for (UInt32 i = 0; i < funcMask.byteSize; ++i)
				{
					BaseDLL.decode_int8(buffer, ref pos, ref funcMask.flags[i]);
				}
				value = funcMask;
				return true;
			}
			else if (field.FieldType == typeof(BootMaskProperty))
			{
				BootMaskProperty bootMask = new BootMaskProperty();
				for (UInt32 i = 0; i < bootMask.byteSize; ++i)
				{
					BaseDLL.decode_int8(buffer, ref pos, ref bootMask.flags[i]);
				}
				value = bootMask;
				return true;
			}
			else if (field.FieldType == typeof(Dictionary<string, CounterInfo>))
			{
				Dictionary<string, CounterInfo> counterMgr = new Dictionary<string, CounterInfo>();
				while (true)
				{
					byte flag = 0;
					BaseDLL.decode_int8(buffer, ref pos, ref flag);
					if (flag == 0)
					{
						break;
					}

					CounterInfo info = new CounterInfo();
					UInt16 nameLen = 0;
					BaseDLL.decode_uint16(buffer, ref pos, ref nameLen);
					byte[] nameBytes = new byte[nameLen];
					for (int i = 0; i < nameLen; i++)
					{
						BaseDLL.decode_int8(buffer, ref pos, ref nameBytes[i]);
					}
					info.name = StringHelper.BytesToString(nameBytes);
					BaseDLL.decode_uint32(buffer, ref pos, ref info.value);

					counterMgr.Add(info.name, info);
				}

				value = counterMgr;
				return true;
			}
			else if (field.FieldType == typeof(PlayerAvatar))
			{
				PlayerAvatar avatar = new PlayerAvatar();
				avatar.decode(buffer, ref pos);
				value = avatar;
				return true;
			}
			else if (field.FieldType == typeof(Vip))
			{
				Vip vip = new Vip();
				vip.decode(buffer, ref pos);
				value = vip;
				return true;
			}
			else if (field.FieldType == typeof(SceneRetinue))
			{
				SceneRetinue retinue = new SceneRetinue();
				retinue.decode(buffer, ref pos);
				value = retinue;
				return true;
			}
			else if (field.FieldType == typeof(ScenePet))
			{
				ScenePet pet = new ScenePet();
				pet.decode(buffer, ref pos);
				value = pet;
				return true;
			}
			else if (field.FieldType == typeof(GuildBattleMaskProperty))
			{
				GuildBattleMaskProperty bootMask = new GuildBattleMaskProperty();
				for (UInt32 i = 0; i < bootMask.byteSize; ++i)
				{
					BaseDLL.decode_int8(buffer, ref pos, ref bootMask.flags[i]);
				}
				value = bootMask;
				return true;
			}
			else if (field.FieldType == typeof(DailyTaskMaskProperty))
			{
				DailyTaskMaskProperty mask = new DailyTaskMaskProperty();
				for (UInt32 i = 0; i < mask.byteSize; ++i)
				{
					BaseDLL.decode_int8(buffer, ref pos, ref mask.flags[i]);
				}
				value = mask;
				return true;
			}
			else if (field.FieldType == typeof(AchievementMaskProperty))
			{
				AchievementMaskProperty mask = new AchievementMaskProperty();
				for (UInt32 i = 0; i < mask.byteSize; ++i)
				{
					BaseDLL.decode_int8(buffer, ref pos, ref mask.flags[i]);
				}
				value = mask;
				return true;
			}
			else if (field.FieldType == typeof(MoneyManageMaskProperty))
			{
				MoneyManageMaskProperty mask = new MoneyManageMaskProperty();
				for (UInt32 i = 0; i < mask.byteSize; ++i)
				{
					BaseDLL.decode_int8(buffer, ref pos, ref mask.flags[i]);
				}
				value = mask;
				return true;
			}
			else if (field.FieldType == typeof(ScoreWarMaskProperty))
			{
				ScoreWarMaskProperty mask = new ScoreWarMaskProperty();
				for(UInt32 i = 0; i < mask.byteSize; ++i)
				{
					BaseDLL.decode_int8(buffer, ref pos, ref mask.flags[i]);
				}
				value = mask;
				return true;
			}
			else if (field.FieldType == typeof(FatigueInfo))
			{
				FatigueInfo info = new FatigueInfo();
				BaseDLL.decode_uint16(buffer, ref pos, ref info.fatigue);
				BaseDLL.decode_uint16(buffer, ref pos, ref info.maxFatigue);
				value = info;
				return true;
			}
			else if (field.FieldType == typeof(List<byte>))
			{
				List<byte> infos = new List<byte>();

				while (true)
				{
					byte val = 0;
					BaseDLL.decode_int8(buffer, ref pos, ref val);
					if (val == 0)
					{
						break;
					}
					infos.Add(val);
				}
				value = infos;
				return true;
			}
			else if (field.FieldType == typeof(List<UInt32>))
			{
				List<UInt32> sets = new List<UInt32>();

				byte len = 0;
				BaseDLL.decode_int8(buffer, ref pos, ref len);

				for (int i = 0; i < len; ++i)
				{
					UInt32 val = 0;
					BaseDLL.decode_uint32(buffer, ref pos, ref val);
					sets.Add(val);
				}

				value = sets;

				return true;
			}
			else if (field.FieldType == typeof(List<UInt64>))
			{
				List<UInt64> sets = new List<UInt64>();

				short len = 0;
				BaseDLL.decode_int16(buffer, ref pos, ref len);

				for (int i = 0; i < len; ++i)
				{
					UInt64 val = 0;
					BaseDLL.decode_uint64(buffer, ref pos, ref val);
					sets.Add(val);
				}

				value = sets;

				return true;
			}
			else if (field.FieldType == typeof(PlayerWearedTitleInfo))
            {
                PlayerWearedTitleInfo info = new PlayerWearedTitleInfo();
                info.decode(buffer, ref pos);
                value = info;
                return true;
            }else if(field.FieldType==typeof(SkillMgr))
            {
                SkillMgr skillMgr = new SkillMgr();
                skillMgr.decode(buffer, ref pos);
                value = skillMgr;
                return true;
            }else if(field.FieldType==typeof(Sp))
            {
                Sp sp = new Sp();
                sp.decode(buffer, ref pos);
                value = sp;
                return true;
            }
			
            return false;
        }
    }
}