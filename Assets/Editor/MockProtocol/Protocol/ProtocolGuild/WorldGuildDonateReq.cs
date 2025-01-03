using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  请求捐赠
	/// </summary>
	[AdvancedInspector.Descriptor(" 请求捐赠", " 请求捐赠")]
	public class WorldGuildDonateReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601928;
		public UInt32 Sequence;
		/// <summary>
		///  捐赠类型（对应枚举GuildDonateType）
		/// </summary>
		[AdvancedInspector.Descriptor(" 捐赠类型（对应枚举GuildDonateType）", " 捐赠类型（对应枚举GuildDonateType）")]
		public byte type;
		/// <summary>
		///  次数
		/// </summary>
		[AdvancedInspector.Descriptor(" 次数", " 次数")]
		public byte num;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, type);
			BaseDLL.encode_int8(buffer, ref pos_, num);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref type);
			BaseDLL.decode_int8(buffer, ref pos_, ref num);
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
