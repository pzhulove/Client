using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 删除
	/// </summary>
	/// <summary>
	/// world->client 更新头衔数据
	/// </summary>
	[AdvancedInspector.Descriptor("world->client 更新头衔数据", "world->client 更新头衔数据")]
	public class WorldNewTitleUpdateData : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 609207;
		public UInt32 Sequence;

		public PlayerTitleInfo[] updates = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)updates.Length);
			for(int i = 0; i < updates.Length; i++)
			{
				updates[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 updatesCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref updatesCnt);
			updates = new PlayerTitleInfo[updatesCnt];
			for(int i = 0; i < updates.Length; i++)
			{
				updates[i] = new PlayerTitleInfo();
				updates[i].decode(buffer, ref pos_);
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
