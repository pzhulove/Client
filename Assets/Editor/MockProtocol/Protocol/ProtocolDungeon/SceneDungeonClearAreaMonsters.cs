using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 玩家杀死怪物返回
	/// </summary>
	[AdvancedInspector.Descriptor("玩家杀死怪物返回", "玩家杀死怪物返回")]
	public class SceneDungeonClearAreaMonsters : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 506821;
		public UInt32 Sequence;
		/// <summary>
		///  使用时间(ms)
		/// </summary>
		[AdvancedInspector.Descriptor(" 使用时间(ms)", " 使用时间(ms)")]
		public UInt32 usedTime;
		/// <summary>
		///  剩余蓝量
		/// </summary>
		[AdvancedInspector.Descriptor(" 剩余蓝量", " 剩余蓝量")]
		public UInt32 remainMp;

		public byte[] md5 = new byte[16];
		/// <summary>
		///  剩余血量
		/// </summary>
		[AdvancedInspector.Descriptor(" 剩余血量", " 剩余血量")]
		public UInt32 remainHp;
		/// <summary>
		///  最后一帧
		/// </summary>
		[AdvancedInspector.Descriptor(" 最后一帧", " 最后一帧")]
		public UInt32 lastFrame;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, usedTime);
			BaseDLL.encode_uint32(buffer, ref pos_, remainMp);
			for(int i = 0; i < md5.Length; i++)
			{
				BaseDLL.encode_int8(buffer, ref pos_, md5[i]);
			}
			BaseDLL.encode_uint32(buffer, ref pos_, remainHp);
			BaseDLL.encode_uint32(buffer, ref pos_, lastFrame);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref usedTime);
			BaseDLL.decode_uint32(buffer, ref pos_, ref remainMp);
			for(int i = 0; i < md5.Length; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref md5[i]);
			}
			BaseDLL.decode_uint32(buffer, ref pos_, ref remainHp);
			BaseDLL.decode_uint32(buffer, ref pos_, ref lastFrame);
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
