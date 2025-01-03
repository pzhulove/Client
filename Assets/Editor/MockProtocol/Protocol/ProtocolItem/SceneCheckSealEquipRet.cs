using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  装备uid
	/// </summary>
	[AdvancedInspector.Descriptor(" 装备uid", " 装备uid")]
	public class SceneCheckSealEquipRet : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 500940;
		public UInt32 Sequence;

		public UInt32 code;
		/// <summary>
		/// 返回码
		/// </summary>
		[AdvancedInspector.Descriptor("返回码", "返回码")]
		public UInt32 matID;
		/// <summary>
		/// 材料ID
		/// </summary>
		[AdvancedInspector.Descriptor("材料ID", "材料ID")]
		public UInt16 needNum;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, code);
			BaseDLL.encode_uint32(buffer, ref pos_, matID);
			BaseDLL.encode_uint16(buffer, ref pos_, needNum);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			BaseDLL.decode_uint32(buffer, ref pos_, ref matID);
			BaseDLL.decode_uint16(buffer, ref pos_, ref needNum);
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
