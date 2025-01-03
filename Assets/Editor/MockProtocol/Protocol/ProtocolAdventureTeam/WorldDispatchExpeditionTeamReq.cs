using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  client->world 派遣远征队请求
	/// </summary>
	[AdvancedInspector.Descriptor(" client->world 派遣远征队请求", " client->world 派遣远征队请求")]
	public class WorldDispatchExpeditionTeamReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 608615;
		public UInt32 Sequence;
		/// <summary>
		///  地图id
		/// </summary>
		[AdvancedInspector.Descriptor(" 地图id", " 地图id")]
		public byte mapId;
		/// <summary>
		///  远征成员(角色id列表)
		/// </summary>
		[AdvancedInspector.Descriptor(" 远征成员(角色id列表)", " 远征成员(角色id列表)")]
		public UInt64[] members = new UInt64[0];
		/// <summary>
		///  远征时间(持续时间 小时)
		/// </summary>
		[AdvancedInspector.Descriptor(" 远征时间(持续时间 小时)", " 远征时间(持续时间 小时)")]
		public UInt32 housOfduration;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, mapId);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)members.Length);
			for(int i = 0; i < members.Length; i++)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, members[i]);
			}
			BaseDLL.encode_uint32(buffer, ref pos_, housOfduration);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref mapId);
			UInt16 membersCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref membersCnt);
			members = new UInt64[membersCnt];
			for(int i = 0; i < members.Length; i++)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref members[i]);
			}
			BaseDLL.decode_uint32(buffer, ref pos_, ref housOfduration);
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
