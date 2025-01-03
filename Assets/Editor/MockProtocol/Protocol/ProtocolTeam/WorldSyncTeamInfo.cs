using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  同步队伍信息
	/// </summary>
	[AdvancedInspector.Descriptor(" 同步队伍信息", " 同步队伍信息")]
	public class WorldSyncTeamInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601601;
		public UInt32 Sequence;
		/// <summary>
		///  队伍编号
		/// </summary>
		[AdvancedInspector.Descriptor(" 队伍编号", " 队伍编号")]
		public UInt16 id;
		/// <summary>
		///  队伍目标
		/// </summary>
		[AdvancedInspector.Descriptor(" 队伍目标", " 队伍目标")]
		public UInt32 target;
		/// <summary>
		///  是否自动同意
		/// </summary>
		[AdvancedInspector.Descriptor(" 是否自动同意", " 是否自动同意")]
		public byte autoAgree;
		/// <summary>
		///  队长
		/// </summary>
		[AdvancedInspector.Descriptor(" 队长", " 队长")]
		public UInt64 master;
		/// <summary>
		///  队伍成员链表
		/// </summary>
		[AdvancedInspector.Descriptor(" 队伍成员链表", " 队伍成员链表")]
		public TeammemberInfo[] members = null;
		/// <summary>
		/// 队伍选项(对应TeamOption的位组合)
		/// </summary>
		[AdvancedInspector.Descriptor("队伍选项(对应TeamOption的位组合)", "队伍选项(对应TeamOption的位组合)")]
		public UInt32 options;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, id);
			BaseDLL.encode_uint32(buffer, ref pos_, target);
			BaseDLL.encode_int8(buffer, ref pos_, autoAgree);
			BaseDLL.encode_uint64(buffer, ref pos_, master);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)members.Length);
			for(int i = 0; i < members.Length; i++)
			{
				members[i].encode(buffer, ref pos_);
			}
			BaseDLL.encode_uint32(buffer, ref pos_, options);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint16(buffer, ref pos_, ref id);
			BaseDLL.decode_uint32(buffer, ref pos_, ref target);
			BaseDLL.decode_int8(buffer, ref pos_, ref autoAgree);
			BaseDLL.decode_uint64(buffer, ref pos_, ref master);
			UInt16 membersCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref membersCnt);
			members = new TeammemberInfo[membersCnt];
			for(int i = 0; i < members.Length; i++)
			{
				members[i] = new TeammemberInfo();
				members[i].decode(buffer, ref pos_);
			}
			BaseDLL.decode_uint32(buffer, ref pos_, ref options);
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
