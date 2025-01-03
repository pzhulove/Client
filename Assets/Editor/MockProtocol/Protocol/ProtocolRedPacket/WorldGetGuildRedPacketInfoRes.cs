using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// world->client 获取公会红包信息返回
	/// </summary>
	[AdvancedInspector.Descriptor("world->client 获取公会红包信息返回", "world->client 获取公会红包信息返回")]
	public class WorldGetGuildRedPacketInfoRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 607314;
		public UInt32 Sequence;

		public UInt32 code;

		public GuildRedPacketSpecInfo[] infos = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, code);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)infos.Length);
			for(int i = 0; i < infos.Length; i++)
			{
				infos[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			UInt16 infosCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref infosCnt);
			infos = new GuildRedPacketSpecInfo[infosCnt];
			for(int i = 0; i < infos.Length; i++)
			{
				infos[i] = new GuildRedPacketSpecInfo();
				infos[i].decode(buffer, ref pos_);
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
