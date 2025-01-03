using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 补签
	/// </summary>
	[AdvancedInspector.Descriptor("补签", "补签")]
	public class SceneActiveTaskSubmitRp : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501131;
		public UInt32 Sequence;

		public UInt32[] taskId = new UInt32[0];

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)taskId.Length);
			for(int i = 0; i < taskId.Length; i++)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, taskId[i]);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 taskIdCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref taskIdCnt);
			taskId = new UInt32[taskIdCnt];
			for(int i = 0; i < taskId.Length; i++)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref taskId[i]);
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
