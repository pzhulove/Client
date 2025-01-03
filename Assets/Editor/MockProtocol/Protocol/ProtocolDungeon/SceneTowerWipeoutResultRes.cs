using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  死亡之塔扫荡奖励返回
	/// </summary>
	[AdvancedInspector.Descriptor(" 死亡之塔扫荡奖励返回", " 死亡之塔扫荡奖励返回")]
	public class SceneTowerWipeoutResultRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 507204;
		public UInt32 Sequence;

		public UInt32 result;

		public SceneDungeonDropItem[] items = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, result);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)items.Length);
			for(int i = 0; i < items.Length; i++)
			{
				items[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			UInt16 itemsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref itemsCnt);
			items = new SceneDungeonDropItem[itemsCnt];
			for(int i = 0; i < items.Length; i++)
			{
				items[i] = new SceneDungeonDropItem();
				items[i].decode(buffer, ref pos_);
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
