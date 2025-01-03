using System;
using System.Collections.Generic;
using System.Reflection;

namespace Protocol
{

	enum PetObjectAttr
	{
		POA_INVALID,			//无效
		POA_LEVEL,				//等级	UInt16
		POA_EXP,				//经验	UInt32
		POA_HUNGER,				//饥饿值	UInt16
		POA_SKILL_INDEX,		//技能索引	UInt8
		POA_GOLD_FEED_COUNT,	//金币喂食次数	UInt8
		POA_POINT_FEED_COUNT,   //点卷喂食次数	UInt8
		POA_SELECT_SKILL_COUNT, //技能重选次数	UInt8
		POA_PET_SCORE,			//宠物评分	UInt32
	};

	public class Pet : StreamObject
	{
		public UInt64 id;
		public UInt32 dataId;
		[SProperty((int)PetObjectAttr.POA_LEVEL)]
		public UInt16 level;
		[SProperty((int)PetObjectAttr.POA_EXP)]
		public UInt32 exp;
		[SProperty((int)PetObjectAttr.POA_HUNGER)]
		public UInt16 hunger;
		[SProperty((int)PetObjectAttr.POA_SKILL_INDEX)]
		public byte skillIndex;
		[SProperty((int)PetObjectAttr.POA_GOLD_FEED_COUNT)]
		public byte goldFeedCount;
		[SProperty((int)PetObjectAttr.POA_POINT_FEED_COUNT)]
		public byte pointFeedCount;
		[SProperty((int)PetObjectAttr.POA_SELECT_SKILL_COUNT)]
		public byte selectSkillCount;
		[SProperty((int)PetObjectAttr.POA_PET_SCORE)]
		public UInt32 petScore;
	}

	class PetDecoder
	{
		public static Pet Decode(byte[] buffer, ref int pos, int length)
		{
			Pet pet = new Pet();
			UInt64 uid = 0;
			UInt32 dataId = 0;
			BaseDLL.decode_uint64(buffer, ref pos, ref uid);
			BaseDLL.decode_uint32(buffer, ref pos, ref dataId);
			pet.id = uid;
			pet.dataId = dataId;
			StreamObjectDecoder<Pet>.DecodePropertys(ref pet, buffer, ref pos, length);
			return pet;
		}

		public static List<Pet> DecodeList(byte[] buffer, ref int pos, int length, bool isUpdate = false)
		{
			List<Pet> pets = new List<Pet>();

			while (true)
			{
				UInt64 uid = 0;
				UInt32 dataId = 0;
				BaseDLL.decode_uint64(buffer, ref pos, ref uid);
				if (0 == uid) break;

				if (isUpdate == false)
				{
					BaseDLL.decode_uint32(buffer, ref pos, ref dataId);
				}

				Pet pet = new Pet();
				StreamObjectDecoder<Pet>.DecodePropertys(ref pet, buffer, ref pos, length);

				pet.id = uid;
				pet.dataId = dataId;
				pets.Add(pet);
			}

			return pets;
		}
	}
}
