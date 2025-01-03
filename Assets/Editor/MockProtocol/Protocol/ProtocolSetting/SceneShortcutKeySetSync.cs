using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  游戏快捷键设置同步
	/// </summary>
	[AdvancedInspector.Descriptor(" 游戏快捷键设置同步", " 游戏快捷键设置同步")]
	public class SceneShortcutKeySetSync : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501231;
		public UInt32 Sequence;

		public ShortcutKeyInfo[] infos = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)infos.Length);
			for(int i = 0; i < infos.Length; i++)
			{
				infos[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 infosCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref infosCnt);
			infos = new ShortcutKeyInfo[infosCnt];
			for(int i = 0; i < infos.Length; i++)
			{
				infos[i] = new ShortcutKeyInfo();
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
