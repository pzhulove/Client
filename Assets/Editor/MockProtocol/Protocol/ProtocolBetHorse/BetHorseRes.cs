using System;
using System.Text;

namespace Mock.Protocol
{

	public class BetHorseRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 708302;
		public UInt32 Sequence;
		/// <summary>
		///  当前天气(WeatherType)
		/// </summary>
		[AdvancedInspector.Descriptor(" 当前天气(WeatherType)", " 当前天气(WeatherType)")]
		public UInt32 weather;
		/// <summary>
		///  神秘射手
		/// </summary>
		[AdvancedInspector.Descriptor(" 神秘射手", " 神秘射手")]
		public UInt32 mysteryShooter;
		/// <summary>
		///  赌马阶段
		/// </summary>
		[AdvancedInspector.Descriptor(" 赌马阶段", " 赌马阶段")]
		public UInt32 phase;
		/// <summary>
		///  时间戳
		/// </summary>
		[AdvancedInspector.Descriptor(" 时间戳", " 时间戳")]
		public UInt32 stamp;
		/// <summary>
		///  射手列表
		/// </summary>
		[AdvancedInspector.Descriptor(" 射手列表", " 射手列表")]
		public ShooterInfo[] shooterList = null;
		/// <summary>
		///  地图列表
		/// </summary>
		[AdvancedInspector.Descriptor(" 地图列表", " 地图列表")]
		public MapInfo[] mapList = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, weather);
			BaseDLL.encode_uint32(buffer, ref pos_, mysteryShooter);
			BaseDLL.encode_uint32(buffer, ref pos_, phase);
			BaseDLL.encode_uint32(buffer, ref pos_, stamp);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)shooterList.Length);
			for(int i = 0; i < shooterList.Length; i++)
			{
				shooterList[i].encode(buffer, ref pos_);
			}
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)mapList.Length);
			for(int i = 0; i < mapList.Length; i++)
			{
				mapList[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref weather);
			BaseDLL.decode_uint32(buffer, ref pos_, ref mysteryShooter);
			BaseDLL.decode_uint32(buffer, ref pos_, ref phase);
			BaseDLL.decode_uint32(buffer, ref pos_, ref stamp);
			UInt16 shooterListCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref shooterListCnt);
			shooterList = new ShooterInfo[shooterListCnt];
			for(int i = 0; i < shooterList.Length; i++)
			{
				shooterList[i] = new ShooterInfo();
				shooterList[i].decode(buffer, ref pos_);
			}
			UInt16 mapListCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref mapListCnt);
			mapList = new MapInfo[mapListCnt];
			for(int i = 0; i < mapList.Length; i++)
			{
				mapList[i] = new MapInfo();
				mapList[i].decode(buffer, ref pos_);
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
