using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// scene->client 地牢开宝箱返回
	/// </summary>
	[AdvancedInspector.Descriptor("scene->client 地牢开宝箱返回", "scene->client 地牢开宝箱返回")]
	public class LostDungeonOpenBoxRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 510017;
		public UInt32 Sequence;

		public UInt32 code;

		public ItemReward[] itemVec = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, code);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)itemVec.Length);
			for(int i = 0; i < itemVec.Length; i++)
			{
				itemVec[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			UInt16 itemVecCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref itemVecCnt);
			itemVec = new ItemReward[itemVecCnt];
			for(int i = 0; i < itemVec.Length; i++)
			{
				itemVec[i] = new ItemReward();
				itemVec[i].decode(buffer, ref pos_);
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
