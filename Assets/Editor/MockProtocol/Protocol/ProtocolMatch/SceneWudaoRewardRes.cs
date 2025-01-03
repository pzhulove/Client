using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  领取武道大会奖励返回
	/// </summary>
	[AdvancedInspector.Descriptor(" 领取武道大会奖励返回", " 领取武道大会奖励返回")]
	public class SceneWudaoRewardRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 506709;
		public UInt32 Sequence;

		public UInt32 result;

		public ItemReward[] getItems = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, result);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)getItems.Length);
			for(int i = 0; i < getItems.Length; i++)
			{
				getItems[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			UInt16 getItemsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref getItemsCnt);
			getItems = new ItemReward[getItemsCnt];
			for(int i = 0; i < getItems.Length; i++)
			{
				getItems[i] = new ItemReward();
				getItems[i].decode(buffer, ref pos_);
			}
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
