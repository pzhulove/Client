using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// world->client
	/// </summary>
	[AdvancedInspector.Descriptor("world->client", "world->client")]
	public class WorldAuctionSyncPubPageIds : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 603928;
		public UInt32 Sequence;
		/// <summary>
		/// �����нṹ��id
		/// </summary>
		[AdvancedInspector.Descriptor("�����нṹ��id", "�����нṹ��id")]
		public UInt32[] pageIds = new UInt32[0];
		/// <summary>
		/// ���������ε����б�
		/// </summary>
		[AdvancedInspector.Descriptor("���������ε����б�", "���������ε����б�")]
		public UInt32[] shieldItemList = new UInt32[0];

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)pageIds.Length);
			for(int i = 0; i < pageIds.Length; i++)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, pageIds[i]);
			}
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)shieldItemList.Length);
			for(int i = 0; i < shieldItemList.Length; i++)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, shieldItemList[i]);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 pageIdsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref pageIdsCnt);
			pageIds = new UInt32[pageIdsCnt];
			for(int i = 0; i < pageIds.Length; i++)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref pageIds[i]);
			}
			UInt16 shieldItemListCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref shieldItemListCnt);
			shieldItemList = new UInt32[shieldItemListCnt];
			for(int i = 0; i < shieldItemList.Length; i++)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref shieldItemList[i]);
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
