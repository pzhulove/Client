using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  client->world	公共掉落roll请求
	/// </summary>
	[AdvancedInspector.Descriptor(" client->world	公共掉落roll请求", " client->world	公共掉落roll请求")]
	public class WorldDungeonRollItemReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 606817;
		public UInt32 Sequence;
		/// <summary>
		/// 掉落索引
		/// </summary>
		[AdvancedInspector.Descriptor("掉落索引", "掉落索引")]
		public byte dropIndex;
		/// <summary>
		/// 请求类型
		/// </summary>
		[AdvancedInspector.Descriptor("请求类型", "请求类型")]
		public byte opType;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, dropIndex);
			BaseDLL.encode_int8(buffer, ref pos_, opType);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref dropIndex);
			BaseDLL.decode_int8(buffer, ref pos_, ref opType);
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
