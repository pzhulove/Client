using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  修改系统开关
	/// </summary>
	[AdvancedInspector.Descriptor(" 修改系统开关", " 修改系统开关")]
	public class SceneUpdateServiceSwitch : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501215;
		public UInt32 Sequence;
		/// <summary>
		///  系统类型（对应枚举ServiceType）
		/// </summary>
		[AdvancedInspector.Descriptor(" 系统类型（对应枚举ServiceType）", " 系统类型（对应枚举ServiceType）")]
		public UInt16 type;
		/// <summary>
		///  是否开放
		/// </summary>
		[AdvancedInspector.Descriptor(" 是否开放", " 是否开放")]
		public byte open;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, type);
			BaseDLL.encode_int8(buffer, ref pos_, open);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint16(buffer, ref pos_, ref type);
			BaseDLL.decode_int8(buffer, ref pos_, ref open);
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
