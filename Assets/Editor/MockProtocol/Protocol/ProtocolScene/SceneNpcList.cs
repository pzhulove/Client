using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  npc列表
	/// </summary>
	[AdvancedInspector.Descriptor(" npc列表", " npc列表")]
	public class SceneNpcList : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 500621;
		public UInt32 Sequence;
		/// <summary>
		///  所有场景的npc信息
		/// </summary>
		[AdvancedInspector.Descriptor(" 所有场景的npc信息", " 所有场景的npc信息")]
		public SceneNpcInfo[] infoes = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)infoes.Length);
			for(int i = 0; i < infoes.Length; i++)
			{
				infoes[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 infoesCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref infoesCnt);
			infoes = new SceneNpcInfo[infoesCnt];
			for(int i = 0; i < infoes.Length; i++)
			{
				infoes[i] = new SceneNpcInfo();
				infoes[i].decode(buffer, ref pos_);
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
