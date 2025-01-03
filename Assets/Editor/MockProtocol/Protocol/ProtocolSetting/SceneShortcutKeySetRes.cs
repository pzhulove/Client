using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  游戏快捷键设置返回
	/// </summary>
	[AdvancedInspector.Descriptor(" 游戏快捷键设置返回", " 游戏快捷键设置返回")]
	public class SceneShortcutKeySetRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501230;
		public UInt32 Sequence;

		public UInt32 retCode;

		public ShortcutKeyInfo info = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, retCode);
			info.encode(buffer, ref pos_);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
			info.decode(buffer, ref pos_);
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
