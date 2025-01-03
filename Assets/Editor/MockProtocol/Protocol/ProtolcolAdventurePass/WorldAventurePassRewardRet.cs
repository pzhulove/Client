using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// world -> client 领取通行证等级奖励返回
	/// </summary>
	[AdvancedInspector.Descriptor("world -> client 领取通行证等级奖励返回", "world -> client 领取通行证等级奖励返回")]
	public class WorldAventurePassRewardRet : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 609510;
		public UInt32 Sequence;
		/// <summary>
		/// 请求领取的等级
		/// </summary>
		[AdvancedInspector.Descriptor("请求领取的等级", "请求领取的等级")]
		public UInt32 lv;
		/// <summary>
		/// 领取结果
		/// </summary>
		[AdvancedInspector.Descriptor("领取结果", "领取结果")]
		public UInt32 errorCode;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, lv);
			BaseDLL.encode_uint32(buffer, ref pos_, errorCode);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref lv);
			BaseDLL.decode_uint32(buffer, ref pos_, ref errorCode);
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
