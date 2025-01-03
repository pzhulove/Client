using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// client->scene附魔卡升级请求
	/// </summary>
	[AdvancedInspector.Descriptor("client->scene附魔卡升级请求", "client->scene附魔卡升级请求")]
	public class SceneMagicCardUpgradeReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501055;
		public UInt32 Sequence;

		public UInt64 upgradeUid;

		public UInt64 materialUid;

		public UInt64 equipUid;

		public UInt32 cardId;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, upgradeUid);
			BaseDLL.encode_uint64(buffer, ref pos_, materialUid);
			BaseDLL.encode_uint64(buffer, ref pos_, equipUid);
			BaseDLL.encode_uint32(buffer, ref pos_, cardId);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref upgradeUid);
			BaseDLL.decode_uint64(buffer, ref pos_, ref materialUid);
			BaseDLL.decode_uint64(buffer, ref pos_, ref equipUid);
			BaseDLL.decode_uint32(buffer, ref pos_, ref cardId);
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
