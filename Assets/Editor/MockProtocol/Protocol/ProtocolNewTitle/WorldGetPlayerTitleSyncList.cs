using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 头衔名称
	/// </summary>
	/// <summary>
	/// world->client 同步头衔,上线
	/// </summary>
	[AdvancedInspector.Descriptor("world->client 同步头衔,上线", "world->client 同步头衔,上线")]
	public class WorldGetPlayerTitleSyncList : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 609201;
		public UInt32 Sequence;

		public PlayerTitleInfo[] titles = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)titles.Length);
			for(int i = 0; i < titles.Length; i++)
			{
				titles[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 titlesCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref titlesCnt);
			titles = new PlayerTitleInfo[titlesCnt];
			for(int i = 0; i < titles.Length; i++)
			{
				titles[i] = new PlayerTitleInfo();
				titles[i].decode(buffer, ref pos_);
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
