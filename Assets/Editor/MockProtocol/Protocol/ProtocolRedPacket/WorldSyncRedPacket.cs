using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  登录时同步红包基础信息
	/// </summary>
	[AdvancedInspector.Descriptor(" 登录时同步红包基础信息", " 登录时同步红包基础信息")]
	public class WorldSyncRedPacket : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 607301;
		public UInt32 Sequence;
		/// <summary>
		///  红包基础信息
		/// </summary>
		[AdvancedInspector.Descriptor(" 红包基础信息", " 红包基础信息")]
		public RedPacketBaseEntry[] entrys = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)entrys.Length);
			for(int i = 0; i < entrys.Length; i++)
			{
				entrys[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 entrysCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref entrysCnt);
			entrys = new RedPacketBaseEntry[entrysCnt];
			for(int i = 0; i < entrys.Length; i++)
			{
				entrys[i] = new RedPacketBaseEntry();
				entrys[i].decode(buffer, ref pos_);
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
