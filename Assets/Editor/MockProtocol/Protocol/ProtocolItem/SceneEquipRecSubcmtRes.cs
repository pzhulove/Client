using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// server->client 装备回收提交返回
	/// </summary>
	[AdvancedInspector.Descriptor("server->client 装备回收提交返回", "server->client 装备回收提交返回")]
	public class SceneEquipRecSubcmtRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501009;
		public UInt32 Sequence;
		/// <summary>
		/// 错误码
		/// </summary>
		[AdvancedInspector.Descriptor("错误码", "错误码")]
		public UInt32 code;
		/// <summary>
		/// 装备回收积分
		/// </summary>
		[AdvancedInspector.Descriptor("装备回收积分", "装备回收积分")]
		public EqRecScoreItem[] items = null;
		/// <summary>
		/// 当前总积分
		/// </summary>
		[AdvancedInspector.Descriptor("当前总积分", "当前总积分")]
		public UInt32 score;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, code);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)items.Length);
			for(int i = 0; i < items.Length; i++)
			{
				items[i].encode(buffer, ref pos_);
			}
			BaseDLL.encode_uint32(buffer, ref pos_, score);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			UInt16 itemsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref itemsCnt);
			items = new EqRecScoreItem[itemsCnt];
			for(int i = 0; i < items.Length; i++)
			{
				items[i] = new EqRecScoreItem();
				items[i].decode(buffer, ref pos_);
			}
			BaseDLL.decode_uint32(buffer, ref pos_, ref score);
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
