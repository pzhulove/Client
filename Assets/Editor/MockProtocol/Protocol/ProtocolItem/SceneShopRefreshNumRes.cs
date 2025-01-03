using System;
using System.Text;

namespace Mock.Protocol
{

	public class SceneShopRefreshNumRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 500957;
		public UInt32 Sequence;

		public byte shopId;

		public byte restRefreshNum;

		public byte maxRefreshNum;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, shopId);
			BaseDLL.encode_int8(buffer, ref pos_, restRefreshNum);
			BaseDLL.encode_int8(buffer, ref pos_, maxRefreshNum);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref shopId);
			BaseDLL.decode_int8(buffer, ref pos_, ref restRefreshNum);
			BaseDLL.decode_int8(buffer, ref pos_, ref maxRefreshNum);
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
