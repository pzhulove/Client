using System;
using System.Text;

namespace Mock.Protocol
{

	public class GateSendRoleInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 300301;
		public UInt32 Sequence;

		public RoleInfo[] roles = null;
		/// <summary>
		///  可预约职业
		/// </summary>
		[AdvancedInspector.Descriptor(" 可预约职业", " 可预约职业")]
		public byte[] appointmentOccus = new byte[0];
		/// <summary>
		///  已经创建的预约角色数量
		/// </summary>
		[AdvancedInspector.Descriptor(" 已经创建的预约角色数量", " 已经创建的预约角色数量")]
		public UInt32 appointmentRoleNum;
		/// <summary>
		/// 角色基础栏位
		/// </summary>
		[AdvancedInspector.Descriptor("角色基础栏位", "角色基础栏位")]
		public UInt32 baseRoleField;
		/// <summary>
		/// 可扩展角色栏位
		/// </summary>
		[AdvancedInspector.Descriptor("可扩展角色栏位", "可扩展角色栏位")]
		public UInt32 extensibleRoleField;
		/// <summary>
		/// 可扩展角色解锁栏位
		/// </summary>
		[AdvancedInspector.Descriptor("可扩展角色解锁栏位", "可扩展角色解锁栏位")]
		public UInt32 unlockedExtensibleRoleField;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)roles.Length);
			for(int i = 0; i < roles.Length; i++)
			{
				roles[i].encode(buffer, ref pos_);
			}
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)appointmentOccus.Length);
			for(int i = 0; i < appointmentOccus.Length; i++)
			{
				BaseDLL.encode_int8(buffer, ref pos_, appointmentOccus[i]);
			}
			BaseDLL.encode_uint32(buffer, ref pos_, appointmentRoleNum);
			BaseDLL.encode_uint32(buffer, ref pos_, baseRoleField);
			BaseDLL.encode_uint32(buffer, ref pos_, extensibleRoleField);
			BaseDLL.encode_uint32(buffer, ref pos_, unlockedExtensibleRoleField);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 rolesCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref rolesCnt);
			roles = new RoleInfo[rolesCnt];
			for(int i = 0; i < roles.Length; i++)
			{
				roles[i] = new RoleInfo();
				roles[i].decode(buffer, ref pos_);
			}
			UInt16 appointmentOccusCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref appointmentOccusCnt);
			appointmentOccus = new byte[appointmentOccusCnt];
			for(int i = 0; i < appointmentOccus.Length; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref appointmentOccus[i]);
			}
			BaseDLL.decode_uint32(buffer, ref pos_, ref appointmentRoleNum);
			BaseDLL.decode_uint32(buffer, ref pos_, ref baseRoleField);
			BaseDLL.decode_uint32(buffer, ref pos_, ref extensibleRoleField);
			BaseDLL.decode_uint32(buffer, ref pos_, ref unlockedExtensibleRoleField);
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
