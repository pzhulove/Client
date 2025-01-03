using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  今日比赛结束
	/// </summary>
	[AdvancedInspector.Descriptor(" 今日比赛结束", " 今日比赛结束")]
	public class MapInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  地图id
		/// </summary>
		[AdvancedInspector.Descriptor(" 地图id", " 地图id")]
		public UInt32 id;
		/// <summary>
		///  地形
		/// </summary>
		[AdvancedInspector.Descriptor(" 地形", " 地形")]
		public UInt32 terrain;
		/// <summary>
		///  胜利射手id(0未结束)
		/// </summary>
		[AdvancedInspector.Descriptor(" 胜利射手id(0未结束)", " 胜利射手id(0未结束)")]
		public UInt32 winShooterId;
		/// <summary>
		///  地图上的射手
		/// </summary>
		[AdvancedInspector.Descriptor(" 地图上的射手", " 地图上的射手")]
		public UInt32[] shooter = new UInt32[0];

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, id);
			BaseDLL.encode_uint32(buffer, ref pos_, terrain);
			BaseDLL.encode_uint32(buffer, ref pos_, winShooterId);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)shooter.Length);
			for(int i = 0; i < shooter.Length; i++)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, shooter[i]);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref id);
			BaseDLL.decode_uint32(buffer, ref pos_, ref terrain);
			BaseDLL.decode_uint32(buffer, ref pos_, ref winShooterId);
			UInt16 shooterCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref shooterCnt);
			shooter = new UInt32[shooterCnt];
			for(int i = 0; i < shooter.Length; i++)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref shooter[i]);
			}
		}


		#endregion

	}

}
