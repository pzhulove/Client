using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// server->client 装备赎回刷新时间戳返回
	/// </summary>
	[AdvancedInspector.Descriptor("server->client 装备赎回刷新时间戳返回", "server->client 装备赎回刷新时间戳返回")]
	public class SceneEquipRecRedeemTmRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501015;
		public UInt32 Sequence;
		/// <summary>
		/// 刷新时间戳
		/// </summary>
		[AdvancedInspector.Descriptor("刷新时间戳", "刷新时间戳")]
		public UInt64 timestmap;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, timestmap);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref timestmap);
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
