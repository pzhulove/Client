using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 通知参数
	/// </summary>
	/// <summary>
	/// 初始化通知列表
	/// </summary>
	[AdvancedInspector.Descriptor("初始化通知列表", "初始化通知列表")]
	public class SceneInitNotifyList : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501153;
		public UInt32 Sequence;

		public NotifyInfo[] notifys = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)notifys.Length);
			for(int i = 0; i < notifys.Length; i++)
			{
				notifys[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 notifysCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref notifysCnt);
			notifys = new NotifyInfo[notifysCnt];
			for(int i = 0; i < notifys.Length; i++)
			{
				notifys[i] = new NotifyInfo();
				notifys[i].decode(buffer, ref pos_);
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
