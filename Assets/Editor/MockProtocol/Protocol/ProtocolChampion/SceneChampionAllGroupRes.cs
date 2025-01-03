using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 请求16强之后所有组情况返回	
	/// </summary>
	[AdvancedInspector.Descriptor("请求16强之后所有组情况返回	", "请求16强之后所有组情况返回	")]
	public class SceneChampionAllGroupRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 509833;
		public UInt32 Sequence;

		public ChampionGroup[] groups = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)groups.Length);
			for(int i = 0; i < groups.Length; i++)
			{
				groups[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 groupsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref groupsCnt);
			groups = new ChampionGroup[groupsCnt];
			for(int i = 0; i < groups.Length; i++)
			{
				groups[i] = new ChampionGroup();
				groups[i].decode(buffer, ref pos_);
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
