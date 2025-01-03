using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  任命职位
	/// </summary>
	[AdvancedInspector.Descriptor(" 任命职位", " 任命职位")]
	public class WorldGuildChangePostReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601914;
		public UInt32 Sequence;
		/// <summary>
		/// id
		/// </summary>
		[AdvancedInspector.Descriptor("id", "id")]
		public UInt64 id;
		/// <summary>
		/// 职位
		/// </summary>
		[AdvancedInspector.Descriptor("职位", "职位")]
		public byte post;
		/// <summary>
		/// 被替换的人
		/// </summary>
		[AdvancedInspector.Descriptor("被替换的人", "被替换的人")]
		public UInt64 replacerId;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, id);
			BaseDLL.encode_int8(buffer, ref pos_, post);
			BaseDLL.encode_uint64(buffer, ref pos_, replacerId);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref id);
			BaseDLL.decode_int8(buffer, ref pos_, ref post);
			BaseDLL.decode_uint64(buffer, ref pos_, ref replacerId);
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
