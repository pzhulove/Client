using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  活动页签信息同步
	/// </summary>
	[AdvancedInspector.Descriptor(" 活动页签信息同步", " 活动页签信息同步")]
	public class WorldActivityTabsInfoSync : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 607407;
		public UInt32 Sequence;

		public ActivityTabInfo[] tabsInfo = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)tabsInfo.Length);
			for(int i = 0; i < tabsInfo.Length; i++)
			{
				tabsInfo[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 tabsInfoCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref tabsInfoCnt);
			tabsInfo = new ActivityTabInfo[tabsInfoCnt];
			for(int i = 0; i < tabsInfo.Length; i++)
			{
				tabsInfo[i] = new ActivityTabInfo();
				tabsInfo[i].decode(buffer, ref pos_);
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
