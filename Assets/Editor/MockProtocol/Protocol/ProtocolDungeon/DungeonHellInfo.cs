using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  深渊信息
	/// </summary>
	[AdvancedInspector.Descriptor(" 深渊信息", " 深渊信息")]
	public class DungeonHellInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  模式，对应枚举（DungeonHellMode）
		/// </summary>
		[AdvancedInspector.Descriptor(" 模式，对应枚举（DungeonHellMode）", " 模式，对应枚举（DungeonHellMode）")]
		public byte mode;
		/// <summary>
		///  所在区域
		/// </summary>
		[AdvancedInspector.Descriptor(" 所在区域", " 所在区域")]
		public UInt32 areaId;
		/// <summary>
		///  波次信息
		/// </summary>
		[AdvancedInspector.Descriptor(" 波次信息", " 波次信息")]
		public DungeonHellWaveInfo[] waveInfoes = null;
		/// <summary>
		///  掉落
		/// </summary>
		[AdvancedInspector.Descriptor(" 掉落", " 掉落")]
		public SceneDungeonDropItem[] dropItems = null;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, mode);
			BaseDLL.encode_uint32(buffer, ref pos_, areaId);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)waveInfoes.Length);
			for(int i = 0; i < waveInfoes.Length; i++)
			{
				waveInfoes[i].encode(buffer, ref pos_);
			}
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)dropItems.Length);
			for(int i = 0; i < dropItems.Length; i++)
			{
				dropItems[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref mode);
			BaseDLL.decode_uint32(buffer, ref pos_, ref areaId);
			UInt16 waveInfoesCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref waveInfoesCnt);
			waveInfoes = new DungeonHellWaveInfo[waveInfoesCnt];
			for(int i = 0; i < waveInfoes.Length; i++)
			{
				waveInfoes[i] = new DungeonHellWaveInfo();
				waveInfoes[i].decode(buffer, ref pos_);
			}
			UInt16 dropItemsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref dropItemsCnt);
			dropItems = new SceneDungeonDropItem[dropItemsCnt];
			for(int i = 0; i < dropItems.Length; i++)
			{
				dropItems[i] = new SceneDungeonDropItem();
				dropItems[i].decode(buffer, ref pos_);
			}
		}


		#endregion

	}

}
