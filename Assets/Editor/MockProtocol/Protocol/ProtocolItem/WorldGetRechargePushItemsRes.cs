using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 获取充值推送道具列表返回
	/// </summary>
	[AdvancedInspector.Descriptor("获取充值推送道具列表返回", "获取充值推送道具列表返回")]
	public class WorldGetRechargePushItemsRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 602828;
		public UInt32 Sequence;

		public UInt32 retCode;

		public RechargePushItem[] itemVec = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, retCode);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)itemVec.Length);
			for(int i = 0; i < itemVec.Length; i++)
			{
				itemVec[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
			UInt16 itemVecCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref itemVecCnt);
			itemVec = new RechargePushItem[itemVecCnt];
			for(int i = 0; i < itemVec.Length; i++)
			{
				itemVec[i] = new RechargePushItem();
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