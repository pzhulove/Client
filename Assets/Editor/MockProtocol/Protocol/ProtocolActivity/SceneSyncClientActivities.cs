using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 截止时间
	/// </summary>
	/// <summary>
	///  server->client 同步活动数据
	/// </summary>
	[AdvancedInspector.Descriptor(" server->client 同步活动数据", " server->client 同步活动数据")]
	public class SceneSyncClientActivities : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501136;
		public UInt32 Sequence;

		public ActivityInfo[] activities = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)activities.Length);
			for(int i = 0; i < activities.Length; i++)
			{
				activities[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 activitiesCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref activitiesCnt);
			activities = new ActivityInfo[activitiesCnt];
			for(int i = 0; i < activities.Length; i++)
			{
				activities[i] = new ActivityInfo();
				activities[i].decode(buffer, ref pos_);
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
