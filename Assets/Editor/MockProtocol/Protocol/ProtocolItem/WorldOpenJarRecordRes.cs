using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 返回开罐记录
	/// </summary>
	[AdvancedInspector.Descriptor("返回开罐记录", "返回开罐记录")]
	public class WorldOpenJarRecordRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 600902;
		public UInt32 Sequence;

		public UInt32 jarId;

		public OpenJarRecord[] records = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, jarId);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)records.Length);
			for(int i = 0; i < records.Length; i++)
			{
				records[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref jarId);
			UInt16 recordsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref recordsCnt);
			records = new OpenJarRecord[recordsCnt];
			for(int i = 0; i < records.Length; i++)
			{
				records[i] = new OpenJarRecord();
				records[i].decode(buffer, ref pos_);
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
