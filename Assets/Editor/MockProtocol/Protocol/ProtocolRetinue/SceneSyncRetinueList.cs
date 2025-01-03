using System;
using System.Text;

namespace Mock.Protocol
{

	public class SceneSyncRetinueList : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 507001;
		public UInt32 Sequence;

		public UInt64 id;

		public UInt64[] offRetinueIds = new UInt64[0];

		public RetinueInfo[] retinueList = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, id);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)offRetinueIds.Length);
			for(int i = 0; i < offRetinueIds.Length; i++)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, offRetinueIds[i]);
			}
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)retinueList.Length);
			for(int i = 0; i < retinueList.Length; i++)
			{
				retinueList[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref id);
			UInt16 offRetinueIdsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref offRetinueIdsCnt);
			offRetinueIds = new UInt64[offRetinueIdsCnt];
			for(int i = 0; i < offRetinueIds.Length; i++)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref offRetinueIds[i]);
			}
			UInt16 retinueListCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref retinueListCnt);
			retinueList = new RetinueInfo[retinueListCnt];
			for(int i = 0; i < retinueList.Length; i++)
			{
				retinueList[i] = new RetinueInfo();
				retinueList[i].decode(buffer, ref pos_);
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
