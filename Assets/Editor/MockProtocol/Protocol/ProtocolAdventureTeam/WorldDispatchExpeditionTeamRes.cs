using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  world->client 派遣远征队返回
	/// </summary>
	[AdvancedInspector.Descriptor(" world->client 派遣远征队返回", " world->client 派遣远征队返回")]
	public class WorldDispatchExpeditionTeamRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 608616;
		public UInt32 Sequence;
		/// <summary>
		///  错误码
		/// </summary>
		[AdvancedInspector.Descriptor(" 错误码", " 错误码")]
		public UInt32 resCode;
		/// <summary>
		///  地图id
		/// </summary>
		[AdvancedInspector.Descriptor(" 地图id", " 地图id")]
		public byte mapId;
		/// <summary>
		///  远征状态(对应枚举ExpeditionStatus)
		/// </summary>
		[AdvancedInspector.Descriptor(" 远征状态(对应枚举ExpeditionStatus)", " 远征状态(对应枚举ExpeditionStatus)")]
		public byte expeditionStatus;
		/// <summary>
		///  远征持续时间(小时)
		/// </summary>
		[AdvancedInspector.Descriptor(" 远征持续时间(小时)", " 远征持续时间(小时)")]
		public UInt32 durationOfExpedition;
		/// <summary>
		///  远征结束时间
		/// </summary>
		[AdvancedInspector.Descriptor(" 远征结束时间", " 远征结束时间")]
		public UInt32 endTimeOfExpedition;
		/// <summary>
		///  远征队员信息
		/// </summary>
		[AdvancedInspector.Descriptor(" 远征队员信息", " 远征队员信息")]
		public ExpeditionMemberInfo[] members = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, resCode);
			BaseDLL.encode_int8(buffer, ref pos_, mapId);
			BaseDLL.encode_int8(buffer, ref pos_, expeditionStatus);
			BaseDLL.encode_uint32(buffer, ref pos_, durationOfExpedition);
			BaseDLL.encode_uint32(buffer, ref pos_, endTimeOfExpedition);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)members.Length);
			for(int i = 0; i < members.Length; i++)
			{
				members[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref resCode);
			BaseDLL.decode_int8(buffer, ref pos_, ref mapId);
			BaseDLL.decode_int8(buffer, ref pos_, ref expeditionStatus);
			BaseDLL.decode_uint32(buffer, ref pos_, ref durationOfExpedition);
			BaseDLL.decode_uint32(buffer, ref pos_, ref endTimeOfExpedition);
			UInt16 membersCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref membersCnt);
			members = new ExpeditionMemberInfo[membersCnt];
			for(int i = 0; i < members.Length; i++)
			{
				members[i] = new ExpeditionMemberInfo();
				members[i].decode(buffer, ref pos_);
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
