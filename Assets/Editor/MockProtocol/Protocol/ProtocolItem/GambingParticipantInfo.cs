using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  夺宝参与者数据
	/// </summary>
	[AdvancedInspector.Descriptor(" 夺宝参与者数据", " 夺宝参与者数据")]
	public class GambingParticipantInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  roleid
		/// </summary>
		[AdvancedInspector.Descriptor(" roleid", " roleid")]
		public UInt64 participantId;
		/// <summary>
		///  参与者平台(英文)
		/// </summary>
		[AdvancedInspector.Descriptor(" 参与者平台(英文)", " 参与者平台(英文)")]
		public string participantENPlatform;
		/// <summary>
		///  参与者平台(中文)
		/// </summary>
		[AdvancedInspector.Descriptor(" 参与者平台(中文)", " 参与者平台(中文)")]
		public string participantPlatform;
		/// <summary>
		///  参与者服务器
		/// </summary>
		[AdvancedInspector.Descriptor(" 参与者服务器", " 参与者服务器")]
		public string participantServerName;
		/// <summary>
		///  名字
		/// </summary>
		[AdvancedInspector.Descriptor(" 名字", " 名字")]
		public string participantName;
		/// <summary>
		///  夺宝商品id
		/// </summary>
		[AdvancedInspector.Descriptor(" 夺宝商品id", " 夺宝商品id")]
		public UInt32 gambingItemId;
		/// <summary>
		///  夺宝组id
		/// </summary>
		[AdvancedInspector.Descriptor(" 夺宝组id", " 夺宝组id")]
		public UInt16 groupId;
		/// <summary>
		///  投入份数
		/// </summary>
		[AdvancedInspector.Descriptor(" 投入份数", " 投入份数")]
		public UInt32 investCopies;
		/// <summary>
		///  投入货币id
		/// </summary>
		[AdvancedInspector.Descriptor(" 投入货币id", " 投入货币id")]
		public UInt32 investMoneyId;
		/// <summary>
		///  投入货币数量
		/// </summary>
		[AdvancedInspector.Descriptor(" 投入货币数量", " 投入货币数量")]
		public UInt32 investMoney;
		/// <summary>
		///  夺宝几率
		/// </summary>
		[AdvancedInspector.Descriptor(" 夺宝几率", " 夺宝几率")]
		public string gambingRate;
		/// <summary>
		///  状态(对应枚举 GambingMineStatus)
		/// </summary>
		[AdvancedInspector.Descriptor(" 状态(对应枚举 GambingMineStatus)", " 状态(对应枚举 GambingMineStatus)")]
		public byte status;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, participantId);
			byte[] participantENPlatformBytes = StringHelper.StringToUTF8Bytes(participantENPlatform);
			BaseDLL.encode_string(buffer, ref pos_, participantENPlatformBytes, (UInt16)(buffer.Length - pos_));
			byte[] participantPlatformBytes = StringHelper.StringToUTF8Bytes(participantPlatform);
			BaseDLL.encode_string(buffer, ref pos_, participantPlatformBytes, (UInt16)(buffer.Length - pos_));
			byte[] participantServerNameBytes = StringHelper.StringToUTF8Bytes(participantServerName);
			BaseDLL.encode_string(buffer, ref pos_, participantServerNameBytes, (UInt16)(buffer.Length - pos_));
			byte[] participantNameBytes = StringHelper.StringToUTF8Bytes(participantName);
			BaseDLL.encode_string(buffer, ref pos_, participantNameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_uint32(buffer, ref pos_, gambingItemId);
			BaseDLL.encode_uint16(buffer, ref pos_, groupId);
			BaseDLL.encode_uint32(buffer, ref pos_, investCopies);
			BaseDLL.encode_uint32(buffer, ref pos_, investMoneyId);
			BaseDLL.encode_uint32(buffer, ref pos_, investMoney);
			byte[] gambingRateBytes = StringHelper.StringToUTF8Bytes(gambingRate);
			BaseDLL.encode_string(buffer, ref pos_, gambingRateBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_int8(buffer, ref pos_, status);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref participantId);
			UInt16 participantENPlatformLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref participantENPlatformLen);
			byte[] participantENPlatformBytes = new byte[participantENPlatformLen];
			for(int i = 0; i < participantENPlatformLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref participantENPlatformBytes[i]);
			}
			participantENPlatform = StringHelper.BytesToString(participantENPlatformBytes);
			UInt16 participantPlatformLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref participantPlatformLen);
			byte[] participantPlatformBytes = new byte[participantPlatformLen];
			for(int i = 0; i < participantPlatformLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref participantPlatformBytes[i]);
			}
			participantPlatform = StringHelper.BytesToString(participantPlatformBytes);
			UInt16 participantServerNameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref participantServerNameLen);
			byte[] participantServerNameBytes = new byte[participantServerNameLen];
			for(int i = 0; i < participantServerNameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref participantServerNameBytes[i]);
			}
			participantServerName = StringHelper.BytesToString(participantServerNameBytes);
			UInt16 participantNameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref participantNameLen);
			byte[] participantNameBytes = new byte[participantNameLen];
			for(int i = 0; i < participantNameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref participantNameBytes[i]);
			}
			participantName = StringHelper.BytesToString(participantNameBytes);
			BaseDLL.decode_uint32(buffer, ref pos_, ref gambingItemId);
			BaseDLL.decode_uint16(buffer, ref pos_, ref groupId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref investCopies);
			BaseDLL.decode_uint32(buffer, ref pos_, ref investMoneyId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref investMoney);
			UInt16 gambingRateLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref gambingRateLen);
			byte[] gambingRateBytes = new byte[gambingRateLen];
			for(int i = 0; i < gambingRateLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref gambingRateBytes[i]);
			}
			gambingRate = StringHelper.BytesToString(gambingRateBytes);
			BaseDLL.decode_int8(buffer, ref pos_, ref status);
		}


		#endregion

	}

}
