using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  装备转换请求
	/// </summary>
	[AdvancedInspector.Descriptor(" 装备转换请求", " 装备转换请求")]
	public class SceneEquipConvertReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501092;
		public UInt32 Sequence;
		/// <summary>
		///  装换类型(EquipConvertType)
		/// </summary>
		[AdvancedInspector.Descriptor(" 装换类型(EquipConvertType)", " 装换类型(EquipConvertType)")]
		public byte type;
		/// <summary>
		///  原装备id
		/// </summary>
		[AdvancedInspector.Descriptor(" 原装备id", " 原装备id")]
		public UInt64 sourceEqGuid;
		/// <summary>
		///  目标装备类型id
		/// </summary>
		[AdvancedInspector.Descriptor(" 目标装备类型id", " 目标装备类型id")]
		public UInt32 targetEqTypeId;
		/// <summary>
		///  转换器guid
		/// </summary>
		[AdvancedInspector.Descriptor(" 转换器guid", " 转换器guid")]
		public UInt64 convertorGuid;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, type);
			BaseDLL.encode_uint64(buffer, ref pos_, sourceEqGuid);
			BaseDLL.encode_uint32(buffer, ref pos_, targetEqTypeId);
			BaseDLL.encode_uint64(buffer, ref pos_, convertorGuid);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref type);
			BaseDLL.decode_uint64(buffer, ref pos_, ref sourceEqGuid);
			BaseDLL.decode_uint32(buffer, ref pos_, ref targetEqTypeId);
			BaseDLL.decode_uint64(buffer, ref pos_, ref convertorGuid);
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
