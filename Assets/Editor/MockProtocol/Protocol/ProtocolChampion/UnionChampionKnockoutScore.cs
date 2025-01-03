using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 淘汰赛比分
	/// </summary>
	[AdvancedInspector.Descriptor("淘汰赛比分", "淘汰赛比分")]
	public class UnionChampionKnockoutScore : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 2709813;
		public UInt32 Sequence;

		public KnockoutPlayer[] roles = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)roles.Length);
			for(int i = 0; i < roles.Length; i++)
			{
				roles[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 rolesCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref rolesCnt);
			roles = new KnockoutPlayer[rolesCnt];
			for(int i = 0; i < roles.Length; i++)
			{
				roles[i] = new KnockoutPlayer();
				roles[i].decode(buffer, ref pos_);
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
