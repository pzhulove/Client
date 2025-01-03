using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  client->world 账号商店商品查询请求
	/// </summary>
	[AdvancedInspector.Descriptor(" client->world 账号商店商品查询请求", " client->world 账号商店商品查询请求")]
	public class WorldAccountShopItemQueryReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 608801;
		public UInt32 Sequence;
		/// <summary>
		///  查询索引
		/// </summary>
		[AdvancedInspector.Descriptor(" 查询索引", " 查询索引")]
		public AccountShopQueryIndex queryIndex = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			queryIndex.encode(buffer, ref pos_);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			queryIndex.decode(buffer, ref pos_);
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
