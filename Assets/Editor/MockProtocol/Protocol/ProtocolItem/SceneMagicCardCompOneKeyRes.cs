using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// server->client 附魔卡一键合成返回
	/// </summary>
	[AdvancedInspector.Descriptor("server->client 附魔卡一键合成返回", "server->client 附魔卡一键合成返回")]
	public class SceneMagicCardCompOneKeyRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501058;
		public UInt32 Sequence;

		public UInt32 code;

		public byte endReason;

		public UInt32 compTimes;

		public UInt32 consumeBindGolds;

		public UInt32 comsumeGolds;

		public ItemReward[] items = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, code);
			BaseDLL.encode_int8(buffer, ref pos_, endReason);
			BaseDLL.encode_uint32(buffer, ref pos_, compTimes);
			BaseDLL.encode_uint32(buffer, ref pos_, consumeBindGolds);
			BaseDLL.encode_uint32(buffer, ref pos_, comsumeGolds);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)items.Length);
			for(int i = 0; i < items.Length; i++)
			{
				items[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			BaseDLL.decode_int8(buffer, ref pos_, ref endReason);
			BaseDLL.decode_uint32(buffer, ref pos_, ref compTimes);
			BaseDLL.decode_uint32(buffer, ref pos_, ref consumeBindGolds);
			BaseDLL.decode_uint32(buffer, ref pos_, ref comsumeGolds);
			UInt16 itemsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref itemsCnt);
			items = new ItemReward[itemsCnt];
			for(int i = 0; i < items.Length; i++)
			{
				items[i] = new ItemReward();
				items[i].decode(buffer, ref pos_);
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
