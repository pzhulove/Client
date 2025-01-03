using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  scene->client 升级宝珠返回
	/// </summary>
	[AdvancedInspector.Descriptor(" scene->client 升级宝珠返回", " scene->client 升级宝珠返回")]
	public class SceneUpgradePreciousbeadRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501040;
		public UInt32 Sequence;
		/// <summary>
		/// 返回码
		/// </summary>
		[AdvancedInspector.Descriptor("返回码", "返回码")]
		public UInt32 retCode;
		/// <summary>
		/// 类型 [0]:未镶嵌 [1]:已镶嵌
		/// </summary>
		[AdvancedInspector.Descriptor("类型 [0]:未镶嵌 [1]:已镶嵌", "类型 [0]:未镶嵌 [1]:已镶嵌")]
		public byte mountedType;
		/// <summary>
		/// 装备guid 类型1时设置
		/// </summary>
		[AdvancedInspector.Descriptor("装备guid 类型1时设置", "装备guid 类型1时设置")]
		public UInt64 equipGuid;
		/// <summary>
		/// 升级成功后宝珠id
		/// </summary>
		[AdvancedInspector.Descriptor("升级成功后宝珠id", "升级成功后宝珠id")]
		public UInt32 precId;
		/// <summary>
		/// 附加buff id
		/// </summary>
		[AdvancedInspector.Descriptor("附加buff id", "附加buff id")]
		public UInt32 addBuffId;
		/// <summary>
		/// 升级后新的宝珠uid
		/// </summary>
		[AdvancedInspector.Descriptor("升级后新的宝珠uid", "升级后新的宝珠uid")]
		public UInt64 newPrebeadUid;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, retCode);
			BaseDLL.encode_int8(buffer, ref pos_, mountedType);
			BaseDLL.encode_uint64(buffer, ref pos_, equipGuid);
			BaseDLL.encode_uint32(buffer, ref pos_, precId);
			BaseDLL.encode_uint32(buffer, ref pos_, addBuffId);
			BaseDLL.encode_uint64(buffer, ref pos_, newPrebeadUid);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
			BaseDLL.decode_int8(buffer, ref pos_, ref mountedType);
			BaseDLL.decode_uint64(buffer, ref pos_, ref equipGuid);
			BaseDLL.decode_uint32(buffer, ref pos_, ref precId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref addBuffId);
			BaseDLL.decode_uint64(buffer, ref pos_, ref newPrebeadUid);
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
