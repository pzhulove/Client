using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  账号计数
	/// </summary>
	[AdvancedInspector.Descriptor(" 账号计数", " 账号计数")]
	public class WorldAccountCounterNotify : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 600606;
		public UInt32 Sequence;
		/// <summary>
		///  账号计数列表
		/// </summary>
		[AdvancedInspector.Descriptor(" 账号计数列表", " 账号计数列表")]
		public AccountCounter[] accCounterList = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)accCounterList.Length);
			for(int i = 0; i < accCounterList.Length; i++)
			{
				accCounterList[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 accCounterListCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref accCounterListCnt);
			accCounterList = new AccountCounter[accCounterListCnt];
			for(int i = 0; i < accCounterList.Length; i++)
			{
				accCounterList[i] = new AccountCounter();
				accCounterList[i].decode(buffer, ref pos_);
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
