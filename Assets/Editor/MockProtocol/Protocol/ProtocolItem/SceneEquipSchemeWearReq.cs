using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// client->scene   穿戴装备方案请求
	/// </summary>
	[AdvancedInspector.Descriptor("client->scene   穿戴装备方案请求", "client->scene   穿戴装备方案请求")]
	public class SceneEquipSchemeWearReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501088;
		public UInt32 Sequence;
		/// <summary>
		/// 类型id
		/// </summary>
		[AdvancedInspector.Descriptor("类型id", "类型id")]
		public byte type;
		/// <summary>
		/// 方案id
		/// </summary>
		[AdvancedInspector.Descriptor("方案id", "方案id")]
		public UInt32 id;
		/// <summary>
		/// 是否同步方案
		/// </summary>
		[AdvancedInspector.Descriptor("是否同步方案", "是否同步方案")]
		public byte isSync;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, type);
			BaseDLL.encode_uint32(buffer, ref pos_, id);
			BaseDLL.encode_int8(buffer, ref pos_, isSync);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref type);
			BaseDLL.decode_uint32(buffer, ref pos_, ref id);
			BaseDLL.decode_int8(buffer, ref pos_, ref isSync);
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
