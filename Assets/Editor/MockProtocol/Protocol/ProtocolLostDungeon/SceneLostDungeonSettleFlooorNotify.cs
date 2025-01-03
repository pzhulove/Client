using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 结束
	/// </summary>
	/// <summary>
	/// scene->client 通知客户端爬塔楼层通关
	/// </summary>
	[AdvancedInspector.Descriptor("scene->client 通知客户端爬塔楼层通关", "scene->client 通知客户端爬塔楼层通关")]
	public class SceneLostDungeonSettleFlooorNotify : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 510024;
		public UInt32 Sequence;

		public UInt32 floor;

		public byte battleResult;

		public UInt32 addScore;

		public UInt32 decScore;

		public UInt32 score;

		public UInt32 againItemId;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, floor);
			BaseDLL.encode_int8(buffer, ref pos_, battleResult);
			BaseDLL.encode_uint32(buffer, ref pos_, addScore);
			BaseDLL.encode_uint32(buffer, ref pos_, decScore);
			BaseDLL.encode_uint32(buffer, ref pos_, score);
			BaseDLL.encode_uint32(buffer, ref pos_, againItemId);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref floor);
			BaseDLL.decode_int8(buffer, ref pos_, ref battleResult);
			BaseDLL.decode_uint32(buffer, ref pos_, ref addScore);
			BaseDLL.decode_uint32(buffer, ref pos_, ref decScore);
			BaseDLL.decode_uint32(buffer, ref pos_, ref score);
			BaseDLL.decode_uint32(buffer, ref pos_, ref againItemId);
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
