using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// scene->client   穿戴装备方案返回
	/// </summary>
	[AdvancedInspector.Descriptor("scene->client   穿戴装备方案返回", "scene->client   穿戴装备方案返回")]
	public class SceneEquipSchemeWearRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501089;
		public UInt32 Sequence;
		/// <summary>
		/// 结果
		/// </summary>
		[AdvancedInspector.Descriptor("结果", "结果")]
		public UInt32 code;
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
		/// <summary>
		/// 到期的装备
		/// </summary>
		[AdvancedInspector.Descriptor("到期的装备", "到期的装备")]
		public UInt64[] overdueIds = new UInt64[0];

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, code);
			BaseDLL.encode_int8(buffer, ref pos_, type);
			BaseDLL.encode_uint32(buffer, ref pos_, id);
			BaseDLL.encode_int8(buffer, ref pos_, isSync);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)overdueIds.Length);
			for(int i = 0; i < overdueIds.Length; i++)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, overdueIds[i]);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			BaseDLL.decode_int8(buffer, ref pos_, ref type);
			BaseDLL.decode_uint32(buffer, ref pos_, ref id);
			BaseDLL.decode_int8(buffer, ref pos_, ref isSync);
			UInt16 overdueIdsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref overdueIdsCnt);
			overdueIds = new UInt64[overdueIdsCnt];
			for(int i = 0; i < overdueIds.Length; i++)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref overdueIds[i]);
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
