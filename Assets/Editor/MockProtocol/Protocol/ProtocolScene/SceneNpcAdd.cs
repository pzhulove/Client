using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  新增npc
	/// </summary>
	[AdvancedInspector.Descriptor(" 新增npc", " 新增npc")]
	public class SceneNpcAdd : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 500622;
		public UInt32 Sequence;
		/// <summary>
		///  新增的npc
		/// </summary>
		[AdvancedInspector.Descriptor(" 新增的npc", " 新增的npc")]
		public SceneNpcInfo[] data = null;

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
			data = new SceneNpcInfo[dataCnt];
			for(int i = 0; i < data.Length; i++)
			{
				data[i] = new SceneNpcInfo();
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
