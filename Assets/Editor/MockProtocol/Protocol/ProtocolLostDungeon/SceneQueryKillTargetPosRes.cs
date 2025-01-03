using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// scene->client  查询击杀目标坐标返回
	/// </summary>
	[AdvancedInspector.Descriptor("scene->client  查询击杀目标坐标返回", "scene->client  查询击杀目标坐标返回")]
	public class SceneQueryKillTargetPosRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 510034;
		public UInt32 Sequence;

		public LostDungeonPlayerPos[] playerPos = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)playerPos.Length);
			for(int i = 0; i < playerPos.Length; i++)
			{
				playerPos[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 playerPosCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref playerPosCnt);
			playerPos = new LostDungeonPlayerPos[playerPosCnt];
			for(int i = 0; i < playerPos.Length; i++)
			{
				playerPos[i] = new LostDungeonPlayerPos();
				playerPos[i].decode(buffer, ref pos_);
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
