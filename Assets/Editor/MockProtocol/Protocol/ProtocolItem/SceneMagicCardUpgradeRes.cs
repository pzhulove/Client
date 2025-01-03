using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// scene->client附魔卡升级返回
	/// </summary>
	[AdvancedInspector.Descriptor("scene->client附魔卡升级返回", "scene->client附魔卡升级返回")]
	public class SceneMagicCardUpgradeRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501056;
		public UInt32 Sequence;

		public UInt32 code;

		public UInt32 cardTypeId;

		public byte cardLev;

		public UInt64 cardGuid;

		public UInt64 equipUid;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, code);
			BaseDLL.encode_uint32(buffer, ref pos_, cardTypeId);
			BaseDLL.encode_int8(buffer, ref pos_, cardLev);
			BaseDLL.encode_uint64(buffer, ref pos_, cardGuid);
			BaseDLL.encode_uint64(buffer, ref pos_, equipUid);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			BaseDLL.decode_uint32(buffer, ref pos_, ref cardTypeId);
			BaseDLL.decode_int8(buffer, ref pos_, ref cardLev);
			BaseDLL.decode_uint64(buffer, ref pos_, ref cardGuid);
			BaseDLL.decode_uint64(buffer, ref pos_, ref equipUid);
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
