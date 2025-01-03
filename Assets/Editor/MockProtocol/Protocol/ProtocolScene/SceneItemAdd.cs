using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  新增item
	/// </summary>
	[AdvancedInspector.Descriptor(" 新增item", " 新增item")]
	public class SceneItemAdd : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 500626;
		public UInt32 Sequence;

		public UInt32 battleID;
		/// <summary>
		///  新增的item
		/// </summary>
		[AdvancedInspector.Descriptor(" 新增的item", " 新增的item")]
		public SceneItemInfo[] data = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, battleID);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)data.Length);
			for(int i = 0; i < data.Length; i++)
			{
				data[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref battleID);
			UInt16 dataCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref dataCnt);
			data = new SceneItemInfo[dataCnt];
			for(int i = 0; i < data.Length; i++)
			{
				data[i] = new SceneItemInfo();
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
