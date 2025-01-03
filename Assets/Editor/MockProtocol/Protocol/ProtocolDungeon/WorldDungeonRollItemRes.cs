using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  world->client	公共掉落roll请求返回
	/// </summary>
	[AdvancedInspector.Descriptor(" world->client	公共掉落roll请求返回", " world->client	公共掉落roll请求返回")]
	public class WorldDungeonRollItemRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 606818;
		public UInt32 Sequence;
		/// <summary>
		/// 错误码
		/// </summary>
		[AdvancedInspector.Descriptor("错误码", "错误码")]
		public UInt32 result;
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
		/// <summary>
		/// roll点数
		/// </summary>
		[AdvancedInspector.Descriptor("roll点数", "roll点数")]
		public UInt32 point;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, result);
			BaseDLL.encode_int8(buffer, ref pos_, dropIndex);
			BaseDLL.encode_int8(buffer, ref pos_, opType);
			BaseDLL.encode_uint32(buffer, ref pos_, point);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			BaseDLL.decode_int8(buffer, ref pos_, ref dropIndex);
			BaseDLL.decode_int8(buffer, ref pos_, ref opType);
			BaseDLL.decode_uint32(buffer, ref pos_, ref point);
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
