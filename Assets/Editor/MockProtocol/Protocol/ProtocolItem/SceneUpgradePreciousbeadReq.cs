using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  client->scene 升级宝珠请求
	/// </summary>
	[AdvancedInspector.Descriptor(" client->scene 升级宝珠请求", " client->scene 升级宝珠请求")]
	public class SceneUpgradePreciousbeadReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501039;
		public UInt32 Sequence;
		/// <summary>
		/// 类型 [0]:未镶嵌 [1]:已镶嵌
		/// </summary>
		[AdvancedInspector.Descriptor("类型 [0]:未镶嵌 [1]:已镶嵌", "类型 [0]:未镶嵌 [1]:已镶嵌")]
		public byte mountedType;
		/// <summary>
		/// 宝珠Guid 类型0时设置
		/// </summary>
		[AdvancedInspector.Descriptor("宝珠Guid 类型0时设置", "宝珠Guid 类型0时设置")]
		public UInt64 precGuid;
		/// <summary>
		/// 装备guid 类型1时设置
		/// </summary>
		[AdvancedInspector.Descriptor("装备guid 类型1时设置", "装备guid 类型1时设置")]
		public UInt64 equipGuid;
		/// <summary>
		/// 装备宝珠孔索引 类型1时设置
		/// </summary>
		[AdvancedInspector.Descriptor("装备宝珠孔索引 类型1时设置", "装备宝珠孔索引 类型1时设置")]
		public byte eqPrecHoleIndex;
		/// <summary>
		/// 宝珠id 类型1时设置
		/// </summary>
		[AdvancedInspector.Descriptor("宝珠id 类型1时设置", "宝珠id 类型1时设置")]
		public UInt32 precId;
		/// <summary>
		/// 选择消耗宝珠id 
		/// </summary>
		[AdvancedInspector.Descriptor("选择消耗宝珠id ", "选择消耗宝珠id ")]
		public UInt32 consumePrecId;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, mountedType);
			BaseDLL.encode_uint64(buffer, ref pos_, precGuid);
			BaseDLL.encode_uint64(buffer, ref pos_, equipGuid);
			BaseDLL.encode_int8(buffer, ref pos_, eqPrecHoleIndex);
			BaseDLL.encode_uint32(buffer, ref pos_, precId);
			BaseDLL.encode_uint32(buffer, ref pos_, consumePrecId);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref mountedType);
			BaseDLL.decode_uint64(buffer, ref pos_, ref precGuid);
			BaseDLL.decode_uint64(buffer, ref pos_, ref equipGuid);
			BaseDLL.decode_int8(buffer, ref pos_, ref eqPrecHoleIndex);
			BaseDLL.decode_uint32(buffer, ref pos_, ref precId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref consumePrecId);
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
