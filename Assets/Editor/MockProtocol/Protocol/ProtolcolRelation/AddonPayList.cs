using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 初始化同步代付列表
	/// </summary>
	[AdvancedInspector.Descriptor("初始化同步代付列表", "初始化同步代付列表")]
	public class AddonPayList : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601724;
		public UInt32 Sequence;

		public AddonPayData[] data = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)data.Length);
			for(int i = 0; i < data.Length; i++)
			{
				data[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 dataCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref dataCnt);
			data = new AddonPayData[dataCnt];
			for(int i = 0; i < data.Length; i++)
			{
				data[i] = new AddonPayData();
				data[i].decode(buffer, ref pos_);
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
