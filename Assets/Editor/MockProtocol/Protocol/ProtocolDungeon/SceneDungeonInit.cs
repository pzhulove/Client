using System;
using System.Text;

namespace Mock.Protocol
{

	public class SceneDungeonInit : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 506801;
		public UInt32 Sequence;

		public SceneDungeonInfo[] allInfo = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)allInfo.Length);
			for(int i = 0; i < allInfo.Length; i++)
			{
				allInfo[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 allInfoCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref allInfoCnt);
			allInfo = new SceneDungeonInfo[allInfoCnt];
			for(int i = 0; i < allInfo.Length; i++)
			{
				allInfo[i] = new SceneDungeonInfo();
				allInfo[i].decode(buffer, ref pos_);
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
