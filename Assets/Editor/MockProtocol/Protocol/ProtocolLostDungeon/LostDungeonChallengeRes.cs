using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  挑战迷失地牢返回
	/// </summary>
	[AdvancedInspector.Descriptor(" 挑战迷失地牢返回", " 挑战迷失地牢返回")]
	public class LostDungeonChallengeRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 510002;
		public UInt32 Sequence;
		/// <summary>
		/// 结果
		/// </summary>
		[AdvancedInspector.Descriptor("结果", "结果")]
		public UInt32 code;
		/// <summary>
		/// 匹配的战场id
		/// </summary>
		[AdvancedInspector.Descriptor("匹配的战场id", "匹配的战场id")]
		public UInt32 battleId;
		/// <summary>
		/// 匹配的战场的表id
		/// </summary>
		[AdvancedInspector.Descriptor("匹配的战场的表id", "匹配的战场的表id")]
		public UInt32 battleDataId;
		/// <summary>
		/// 匹配的战场的场景id
		/// </summary>
		[AdvancedInspector.Descriptor("匹配的战场的场景id", "匹配的战场的场景id")]
		public UInt32 sceneId;
		/// <summary>
		/// 层数
		/// </summary>
		[AdvancedInspector.Descriptor("层数", "层数")]
		public UInt32 floor;
		/// <summary>
		///  难度
		/// </summary>
		[AdvancedInspector.Descriptor(" 难度", " 难度")]
		public byte hardType;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, code);
			BaseDLL.encode_uint32(buffer, ref pos_, battleId);
			BaseDLL.encode_uint32(buffer, ref pos_, battleDataId);
			BaseDLL.encode_uint32(buffer, ref pos_, sceneId);
			BaseDLL.encode_uint32(buffer, ref pos_, floor);
			BaseDLL.encode_int8(buffer, ref pos_, hardType);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			BaseDLL.decode_uint32(buffer, ref pos_, ref battleId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref battleDataId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref sceneId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref floor);
			BaseDLL.decode_int8(buffer, ref pos_, ref hardType);
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
