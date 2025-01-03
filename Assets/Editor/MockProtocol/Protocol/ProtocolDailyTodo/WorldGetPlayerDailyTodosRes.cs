using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// world->client 每日必做返回
	/// </summary>
	[AdvancedInspector.Descriptor("world->client 每日必做返回", "world->client 每日必做返回")]
	public class WorldGetPlayerDailyTodosRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 609302;
		public UInt32 Sequence;

		public DailyTodoInfo[] dailyTodos = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)dailyTodos.Length);
			for(int i = 0; i < dailyTodos.Length; i++)
			{
				dailyTodos[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 dailyTodosCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref dailyTodosCnt);
			dailyTodos = new DailyTodoInfo[dailyTodosCnt];
			for(int i = 0; i < dailyTodos.Length; i++)
			{
				dailyTodos[i] = new DailyTodoInfo();
				dailyTodos[i].decode(buffer, ref pos_);
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
