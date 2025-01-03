using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  吃鸡放置陷阱返回
	/// </summary>
	[AdvancedInspector.Descriptor(" 吃鸡放置陷阱返回", " 吃鸡放置陷阱返回")]
	public class SceneBattlePlaceTrapsRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 508934;
		public UInt32 Sequence;

		public UInt32 retCode;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, retCode);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
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
