using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 表id
	/// </summary>
	/// <summary>
	/// world->client 头衔穿戴返回
	/// </summary>
	[AdvancedInspector.Descriptor("world->client 头衔穿戴返回", "world->client 头衔穿戴返回")]
	public class WorldNewTitleTakeUpRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 609203;
		public UInt32 Sequence;

		public UInt32 res;
		/// <summary>
		/// 结果,0成功
		/// </summary>
		[AdvancedInspector.Descriptor("结果,0成功", "结果,0成功")]
		public UInt64 titleGuid;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, res);
			BaseDLL.encode_uint64(buffer, ref pos_, titleGuid);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref res);
			BaseDLL.decode_uint64(buffer, ref pos_, ref titleGuid);
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
