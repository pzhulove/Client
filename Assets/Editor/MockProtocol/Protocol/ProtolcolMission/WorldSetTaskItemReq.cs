using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// client->world 请求提交任务物品
	/// </summary>
	[AdvancedInspector.Descriptor("client->world 请求提交任务物品", "client->world 请求提交任务物品")]
	public class WorldSetTaskItemReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601104;
		public UInt32 Sequence;
		/// <summary>
		///  任务id
		/// </summary>
		[AdvancedInspector.Descriptor(" 任务id", " 任务id")]
		public UInt32 taskId;
		/// <summary>
		///  提交物品
		/// </summary>
		[AdvancedInspector.Descriptor(" 提交物品", " 提交物品")]
		public WorldSetTaskItemStruct[] items = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, taskId);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)items.Length);
			for(int i = 0; i < items.Length; i++)
			{
				items[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref taskId);
			UInt16 itemsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref itemsCnt);
			items = new WorldSetTaskItemStruct[itemsCnt];
			for(int i = 0; i < items.Length; i++)
			{
				items[i] = new WorldSetTaskItemStruct();
				items[i].decode(buffer, ref pos_);
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
