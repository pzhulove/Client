using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// client->server 装备增幅
	/// </summary>
	[AdvancedInspector.Descriptor("client->server 装备增幅", "client->server 装备增幅")]
	public class SceneEquipEnhanceUpgrade : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501060;
		public UInt32 Sequence;
		/// <summary>
		///  装备uid
		/// </summary>
		[AdvancedInspector.Descriptor(" 装备uid", " 装备uid")]
		public UInt64 euqipUid;
		/// <summary>
		///  是否使用保护劵 0为不使用 1为使用 2为使用一次性强化器 3为同时使用保护券和一次性强化器
		/// </summary>
		[AdvancedInspector.Descriptor(" 是否使用保护劵 0为不使用 1为使用 2为使用一次性强化器 3为同时使用保护券和一次性强化器", " 是否使用保护劵 0为不使用 1为使用 2为使用一次性强化器 3为同时使用保护券和一次性强化器")]
		public byte useUnbreak;
		/// <summary>
		///  使用的增幅券
		/// </summary>
		[AdvancedInspector.Descriptor(" 使用的增幅券", " 使用的增幅券")]
		public UInt64 strTickt;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, euqipUid);
			BaseDLL.encode_int8(buffer, ref pos_, useUnbreak);
			BaseDLL.encode_uint64(buffer, ref pos_, strTickt);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref euqipUid);
			BaseDLL.decode_int8(buffer, ref pos_, ref useUnbreak);
			BaseDLL.decode_uint64(buffer, ref pos_, ref strTickt);
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
