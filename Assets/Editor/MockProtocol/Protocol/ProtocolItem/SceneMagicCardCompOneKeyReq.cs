using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// client->scene附魔卡一键合成请求
	/// </summary>
	[AdvancedInspector.Descriptor("client->scene附魔卡一键合成请求", "client->scene附魔卡一键合成请求")]
	public class SceneMagicCardCompOneKeyReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501057;
		public UInt32 Sequence;

		public byte isCompWhite;

		public byte isCompBlue;

		public byte autoUseGold;

		public byte compNotBind;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, isCompWhite);
			BaseDLL.encode_int8(buffer, ref pos_, isCompBlue);
			BaseDLL.encode_int8(buffer, ref pos_, autoUseGold);
			BaseDLL.encode_int8(buffer, ref pos_, compNotBind);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref isCompWhite);
			BaseDLL.decode_int8(buffer, ref pos_, ref isCompBlue);
			BaseDLL.decode_int8(buffer, ref pos_, ref autoUseGold);
			BaseDLL.decode_int8(buffer, ref pos_, ref compNotBind);
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
