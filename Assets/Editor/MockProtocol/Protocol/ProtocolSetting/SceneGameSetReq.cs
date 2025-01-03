using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  设置游戏设置请求
	/// </summary>
	[AdvancedInspector.Descriptor(" 设置游戏设置请求", " 设置游戏设置请求")]
	public class SceneGameSetReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501223;
		public UInt32 Sequence;
		/// <summary>
		///  设置类型
		/// </summary>
		[AdvancedInspector.Descriptor(" 设置类型", " 设置类型")]
		public UInt32 gameSetType;
		/// <summary>
		///  设置值
		/// </summary>
		[AdvancedInspector.Descriptor(" 设置值", " 设置值")]
		public string setValue;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, gameSetType);
			byte[] setValueBytes = StringHelper.StringToUTF8Bytes(setValue);
			BaseDLL.encode_string(buffer, ref pos_, setValueBytes, (UInt16)(buffer.Length - pos_));
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref gameSetType);
			UInt16 setValueLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref setValueLen);
			byte[] setValueBytes = new byte[setValueLen];
			for(int i = 0; i < setValueLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref setValueBytes[i]);
			}
			setValue = StringHelper.BytesToString(setValueBytes);
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
