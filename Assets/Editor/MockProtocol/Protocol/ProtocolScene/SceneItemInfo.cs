using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  场景物品信息
	/// </summary>
	[AdvancedInspector.Descriptor(" 场景物品信息", " 场景物品信息")]
	public class SceneItemInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  场景ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 场景ID", " 场景ID")]
		public UInt32 sceneId;
		/// <summary>
		///  所有item信息
		/// </summary>
		[AdvancedInspector.Descriptor(" 所有item信息", " 所有item信息")]
		public SceneItem[] items = null;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, sceneId);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)items.Length);
			for(int i = 0; i < items.Length; i++)
			{
				items[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref sceneId);
			UInt16 itemsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref itemsCnt);
			items = new SceneItem[itemsCnt];
			for(int i = 0; i < items.Length; i++)
			{
				items[i] = new SceneItem();
				items[i].decode(buffer, ref pos_);
			}
		}


		#endregion

	}

}
