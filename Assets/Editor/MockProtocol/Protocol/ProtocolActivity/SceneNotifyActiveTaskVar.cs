using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 同步活动任务变量更新
	/// </summary>
	[AdvancedInspector.Descriptor("同步活动任务变量更新", "同步活动任务变量更新")]
	public class SceneNotifyActiveTaskVar : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501128;
		public UInt32 Sequence;

		public UInt32 taskId;

		public string key;

		public string val;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, taskId);
			byte[] keyBytes = StringHelper.StringToUTF8Bytes(key);
			BaseDLL.encode_string(buffer, ref pos_, keyBytes, (UInt16)(buffer.Length - pos_));
			byte[] valBytes = StringHelper.StringToUTF8Bytes(val);
			BaseDLL.encode_string(buffer, ref pos_, valBytes, (UInt16)(buffer.Length - pos_));
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref taskId);
			UInt16 keyLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref keyLen);
			byte[] keyBytes = new byte[keyLen];
			for(int i = 0; i < keyLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref keyBytes[i]);
			}
			key = StringHelper.BytesToString(keyBytes);
			UInt16 valLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref valLen);
			byte[] valBytes = new byte[valLen];
			for(int i = 0; i < valLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref valBytes[i]);
			}
			val = StringHelper.BytesToString(valBytes);
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
