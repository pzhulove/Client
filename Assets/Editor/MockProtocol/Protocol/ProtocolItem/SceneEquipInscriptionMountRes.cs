using System;
using System.Text;

namespace Mock.Protocol
{

	public class SceneEquipInscriptionMountRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501078;
		public UInt32 Sequence;
		/// <summary>
		///  装备uid
		/// </summary>
		[AdvancedInspector.Descriptor(" 装备uid", " 装备uid")]
		public UInt64 guid;
		/// <summary>
		///  孔索引
		/// </summary>
		[AdvancedInspector.Descriptor(" 孔索引", " 孔索引")]
		public UInt32 index;
		/// <summary>
		///  铭文uid
		/// </summary>
		[AdvancedInspector.Descriptor(" 铭文uid", " 铭文uid")]
		public UInt64 inscriptionGuid;
		/// <summary>
		///  铭文id
		/// </summary>
		[AdvancedInspector.Descriptor(" 铭文id", " 铭文id")]
		public UInt32 inscriptionId;
		/// <summary>
		///  返回值
		/// </summary>
		[AdvancedInspector.Descriptor(" 返回值", " 返回值")]
		public UInt32 code;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, guid);
			BaseDLL.encode_uint32(buffer, ref pos_, index);
			BaseDLL.encode_uint64(buffer, ref pos_, inscriptionGuid);
			BaseDLL.encode_uint32(buffer, ref pos_, inscriptionId);
			BaseDLL.encode_uint32(buffer, ref pos_, code);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref guid);
			BaseDLL.decode_uint32(buffer, ref pos_, ref index);
			BaseDLL.decode_uint64(buffer, ref pos_, ref inscriptionGuid);
			BaseDLL.decode_uint32(buffer, ref pos_, ref inscriptionId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref code);
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
