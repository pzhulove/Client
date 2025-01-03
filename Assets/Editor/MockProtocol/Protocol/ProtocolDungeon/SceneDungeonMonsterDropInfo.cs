using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  怪物掉落信息
	/// </summary>
	[AdvancedInspector.Descriptor(" 怪物掉落信息", " 怪物掉落信息")]
	public class SceneDungeonMonsterDropInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  怪物ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 怪物ID", " 怪物ID")]
		public UInt32 monsterId;
		/// <summary>
		///  掉落
		/// </summary>
		[AdvancedInspector.Descriptor(" 掉落", " 掉落")]
		public SceneDungeonDropItem[] dropItems = null;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, monsterId);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)dropItems.Length);
			for(int i = 0; i < dropItems.Length; i++)
			{
				dropItems[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref monsterId);
			UInt16 dropItemsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref dropItemsCnt);
			dropItems = new SceneDungeonDropItem[dropItemsCnt];
			for(int i = 0; i < dropItems.Length; i++)
			{
				dropItems[i] = new SceneDungeonDropItem();
				dropItems[i].decode(buffer, ref pos_);
			}
		}


		#endregion

	}

}
