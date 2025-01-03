using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  发送登录推送信息
	/// </summary>
	[AdvancedInspector.Descriptor(" 发送登录推送信息", " 发送登录推送信息")]
	public class GateSendLoginPushInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 300321;
		public UInt32 Sequence;
		/// <summary>
		///  登录推送信息
		/// </summary>
		[AdvancedInspector.Descriptor(" 登录推送信息", " 登录推送信息")]
		public LoginPushInfo[] infos = null;

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
			infos = new LoginPushInfo[infosCnt];
			for(int i = 0; i < infos.Length; i++)
			{
				infos[i] = new LoginPushInfo();
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
