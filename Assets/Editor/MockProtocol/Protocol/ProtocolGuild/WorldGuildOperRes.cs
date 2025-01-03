using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  帮会通用操作返回
	/// </summary>
	[AdvancedInspector.Descriptor(" 帮会通用操作返回", " 帮会通用操作返回")]
	public class WorldGuildOperRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601926;
		public UInt32 Sequence;
		/// <summary>
		///  操作类型（对应枚举GuildOperation）
		/// </summary>
		[AdvancedInspector.Descriptor(" 操作类型（对应枚举GuildOperation）", " 操作类型（对应枚举GuildOperation）")]
		public byte type;
		/// <summary>
		///  结果
		/// </summary>
		[AdvancedInspector.Descriptor(" 结果", " 结果")]
		public UInt32 result;
		/// <summary>
		///  参数1
		/// </summary>
		[AdvancedInspector.Descriptor(" 参数1", " 参数1")]
		public UInt32 param;
		/// <summary>
		///  参数2
		/// </summary>
		[AdvancedInspector.Descriptor(" 参数2", " 参数2")]
		public UInt64 param2;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, type);
			BaseDLL.encode_uint32(buffer, ref pos_, result);
			BaseDLL.encode_uint32(buffer, ref pos_, param);
			BaseDLL.encode_uint64(buffer, ref pos_, param2);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref type);
			BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			BaseDLL.decode_uint32(buffer, ref pos_, ref param);
			BaseDLL.decode_uint64(buffer, ref pos_, ref param2);
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
