using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 全民抢购数据返回
	/// </summary>
	[AdvancedInspector.Descriptor("全民抢购数据返回", "全民抢购数据返回")]
	public class GASWholeBargainRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 707408;
		public UInt32 Sequence;
		/// <summary>
		///  玩家参与次数
		/// </summary>
		[AdvancedInspector.Descriptor(" 玩家参与次数", " 玩家参与次数")]
		public UInt32 playerJoinNum;
		/// <summary>
		///  参与次数
		/// </summary>
		[AdvancedInspector.Descriptor(" 参与次数", " 参与次数")]
		public UInt32 joinNum;
		/// <summary>
		///  最大次数
		/// </summary>
		[AdvancedInspector.Descriptor(" 最大次数", " 最大次数")]
		public UInt32 maxNum;
		/// <summary>
		///  折扣
		/// </summary>
		[AdvancedInspector.Descriptor(" 折扣", " 折扣")]
		public UInt32 discount;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, playerJoinNum);
			BaseDLL.encode_uint32(buffer, ref pos_, joinNum);
			BaseDLL.encode_uint32(buffer, ref pos_, maxNum);
			BaseDLL.encode_uint32(buffer, ref pos_, discount);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref playerJoinNum);
			BaseDLL.decode_uint32(buffer, ref pos_, ref joinNum);
			BaseDLL.decode_uint32(buffer, ref pos_, ref maxNum);
			BaseDLL.decode_uint32(buffer, ref pos_, ref discount);
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
