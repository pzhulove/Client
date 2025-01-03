using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  通知客户端系统开关
	/// </summary>
	[AdvancedInspector.Descriptor(" 通知客户端系统开关", " 通知客户端系统开关")]
	public class GateNotifySysSwitch : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 300324;
		public UInt32 Sequence;

		public SysSwitchCfg[] cfg = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)cfg.Length);
			for(int i = 0; i < cfg.Length; i++)
			{
				cfg[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 cfgCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref cfgCnt);
			cfg = new SysSwitchCfg[cfgCnt];
			for(int i = 0; i < cfg.Length; i++)
			{
				cfg[i] = new SysSwitchCfg();
				cfg[i].decode(buffer, ref pos_);
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
