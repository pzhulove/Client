using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  world->client 查询全部远征地图返回
	/// </summary>
	[AdvancedInspector.Descriptor(" world->client 查询全部远征地图返回", " world->client 查询全部远征地图返回")]
	public class WorldQueryAllExpeditionMapsRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 608622;
		public UInt32 Sequence;
		/// <summary>
		///  错误码
		/// </summary>
		[AdvancedInspector.Descriptor(" 错误码", " 错误码")]
		public UInt32 resCode;
		/// <summary>
		///  地图基本信息集
		/// </summary>
		[AdvancedInspector.Descriptor(" 地图基本信息集", " 地图基本信息集")]
		public ExpeditionMapBaseInfo[] mapBaseInfos = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, resCode);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)mapBaseInfos.Length);
			for(int i = 0; i < mapBaseInfos.Length; i++)
			{
				mapBaseInfos[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref resCode);
			UInt16 mapBaseInfosCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref mapBaseInfosCnt);
			mapBaseInfos = new ExpeditionMapBaseInfo[mapBaseInfosCnt];
			for(int i = 0; i < mapBaseInfos.Length; i++)
			{
				mapBaseInfos[i] = new ExpeditionMapBaseInfo();
				mapBaseInfos[i].decode(buffer, ref pos_);
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
