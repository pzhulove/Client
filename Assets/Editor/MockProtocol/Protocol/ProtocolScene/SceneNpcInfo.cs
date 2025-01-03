using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  场景npc信息
	/// </summary>
	[AdvancedInspector.Descriptor(" 场景npc信息", " 场景npc信息")]
	public class SceneNpcInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  场景ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 场景ID", " 场景ID")]
		public UInt32 sceneId;
		/// <summary>
		///  所有npc信息
		/// </summary>
		[AdvancedInspector.Descriptor(" 所有npc信息", " 所有npc信息")]
		public SceneNpc[] npcs = null;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, sceneId);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)npcs.Length);
			for(int i = 0; i < npcs.Length; i++)
			{
				npcs[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref sceneId);
			UInt16 npcsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref npcsCnt);
			npcs = new SceneNpc[npcsCnt];
			for(int i = 0; i < npcs.Length; i++)
			{
				npcs[i] = new SceneNpc();
				npcs[i].decode(buffer, ref pos_);
			}
		}


		#endregion

	}

}
