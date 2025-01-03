using System;
using System.Text;

namespace Mock.Protocol
{

	public class WorldDigMapInfoRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 608214;
		public UInt32 Sequence;

		public UInt32 result;

		public DigMapInfo[] digMapInfos = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, result);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)digMapInfos.Length);
			for(int i = 0; i < digMapInfos.Length; i++)
			{
				digMapInfos[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			UInt16 digMapInfosCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref digMapInfosCnt);
			digMapInfos = new DigMapInfo[digMapInfosCnt];
			for(int i = 0; i < digMapInfos.Length; i++)
			{
				digMapInfos[i] = new DigMapInfo();
				digMapInfos[i].decode(buffer, ref pos_);
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
