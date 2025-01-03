using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// Scene->Client 请求16强表返回
	/// </summary>
	[AdvancedInspector.Descriptor("Scene->Client 请求16强表返回", "Scene->Client 请求16强表返回")]
	public class SceneChampion16TableRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 509809;
		public UInt32 Sequence;

		public ChampionTop16Player[] playerVec = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)playerVec.Length);
			for(int i = 0; i < playerVec.Length; i++)
			{
				playerVec[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 playerVecCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref playerVecCnt);
			playerVec = new ChampionTop16Player[playerVecCnt];
			for(int i = 0; i < playerVec.Length; i++)
			{
				playerVec[i] = new ChampionTop16Player();
				playerVec[i].decode(buffer, ref pos_);
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
