using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// world -> client 购买冒险通行证返回
	/// </summary>
	[AdvancedInspector.Descriptor("world -> client 购买冒险通行证返回", "world -> client 购买冒险通行证返回")]
	public class WorldAventurePassBuyRet : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 609504;
		public UInt32 Sequence;
		/// <summary>
		/// 冒险通行证类型
		/// </summary>
		[AdvancedInspector.Descriptor("冒险通行证类型", "冒险通行证类型")]
		public byte type;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, type);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref type);
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
