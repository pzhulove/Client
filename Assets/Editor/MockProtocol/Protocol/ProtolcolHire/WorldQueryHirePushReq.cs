using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 查询或设置招募是否已推送
	/// </summary>
	[AdvancedInspector.Descriptor("查询或设置招募是否已推送", "查询或设置招募是否已推送")]
	public class WorldQueryHirePushReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601796;
		public UInt32 Sequence;
		/// <summary>
		/// 0 是查询 1是设置
		/// </summary>
		[AdvancedInspector.Descriptor("0 是查询 1是设置", "0 是查询 1是设置")]
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
