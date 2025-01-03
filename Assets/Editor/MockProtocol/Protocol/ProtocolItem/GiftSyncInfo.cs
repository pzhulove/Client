using System;
using System.Text;

namespace Mock.Protocol
{

	public class GiftSyncInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt32 id;

		public UInt32 itemId;

		public UInt32 itemNum;

		public byte[] recommendJobs = new byte[0];

		public UInt32 weight;

		public UInt32[] validLevels = new UInt32[0];
		/// <summary>
		/// 装备类型，对应枚举EquipType
		/// </summary>
		[AdvancedInspector.Descriptor("装备类型，对应枚举EquipType", "装备类型，对应枚举EquipType")]
		public byte equipType;

		public UInt32 strengthen;

		public byte isTimeLimit;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, id);
			BaseDLL.encode_uint32(buffer, ref pos_, itemId);
			BaseDLL.encode_uint32(buffer, ref pos_, itemNum);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)recommendJobs.Length);
			for(int i = 0; i < recommendJobs.Length; i++)
			{
				BaseDLL.encode_int8(buffer, ref pos_, recommendJobs[i]);
			}
			BaseDLL.encode_uint32(buffer, ref pos_, weight);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)validLevels.Length);
			for(int i = 0; i < validLevels.Length; i++)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, validLevels[i]);
			}
			BaseDLL.encode_int8(buffer, ref pos_, equipType);
			BaseDLL.encode_uint32(buffer, ref pos_, strengthen);
			BaseDLL.encode_int8(buffer, ref pos_, isTimeLimit);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref id);
			BaseDLL.decode_uint32(buffer, ref pos_, ref itemId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref itemNum);
			UInt16 recommendJobsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref recommendJobsCnt);
			recommendJobs = new byte[recommendJobsCnt];
			for(int i = 0; i < recommendJobs.Length; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref recommendJobs[i]);
			}
			BaseDLL.decode_uint32(buffer, ref pos_, ref weight);
			UInt16 validLevelsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref validLevelsCnt);
			validLevels = new UInt32[validLevelsCnt];
			for(int i = 0; i < validLevels.Length; i++)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref validLevels[i]);
			}
			BaseDLL.decode_int8(buffer, ref pos_, ref equipType);
			BaseDLL.decode_uint32(buffer, ref pos_, ref strengthen);
			BaseDLL.decode_int8(buffer, ref pos_, ref isTimeLimit);
		}


		#endregion

	}

}
