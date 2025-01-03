using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  初始化淘汰赛对战列表
	/// </summary>
	[AdvancedInspector.Descriptor(" 初始化淘汰赛对战列表", " 初始化淘汰赛对战列表")]
	public class WorldPremiumLeagueBattleInfoInit : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 607713;
		public UInt32 Sequence;

		public CLPremiumLeagueBattle[] battles = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)battles.Length);
			for(int i = 0; i < battles.Length; i++)
			{
				battles[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 battlesCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref battlesCnt);
			battles = new CLPremiumLeagueBattle[battlesCnt];
			for(int i = 0; i < battles.Length; i++)
			{
				battles[i] = new CLPremiumLeagueBattle();
				battles[i].decode(buffer, ref pos_);
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
