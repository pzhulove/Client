using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// client->world 购买通行证等级
	/// </summary>
	[AdvancedInspector.Descriptor("client->world 购买通行证等级", "client->world 购买通行证等级")]
	public class WorldAventurePassBuyLvReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 609505;
		public UInt32 Sequence;
		/// <summary>
		/// 购买的等级档次
		/// </summary>
		[AdvancedInspector.Descriptor("购买的等级档次", "购买的等级档次")]
		public UInt32 lv;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, lv);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref lv);
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
