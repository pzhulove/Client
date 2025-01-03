using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  场景item列表
	/// </summary>
	[AdvancedInspector.Descriptor(" 场景item列表", " 场景item列表")]
	public class SceneItemList : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 500625;
		public UInt32 Sequence;

		public UInt32 battleID;
		/// <summary>
		///  所有场景的物品信息
		/// </summary>
		[AdvancedInspector.Descriptor(" 所有场景的物品信息", " 所有场景的物品信息")]
		public SceneItemInfo[] infoes = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, battleID);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)infoes.Length);
			for(int i = 0; i < infoes.Length; i++)
			{
				infoes[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref battleID);
			UInt16 infoesCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref infoesCnt);
			infoes = new SceneItemInfo[infoesCnt];
			for(int i = 0; i < infoes.Length; i++)
			{
				infoes[i] = new SceneItemInfo();
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
