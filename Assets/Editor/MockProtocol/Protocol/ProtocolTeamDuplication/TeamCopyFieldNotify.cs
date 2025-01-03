using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  据点通知
	/// </summary>
	[AdvancedInspector.Descriptor(" 据点通知", " 据点通知")]
	public class TeamCopyFieldNotify : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 1100020;
		public UInt32 Sequence;
		/// <summary>
		///  据点列表(已存在的据点更新，新据点增加)
		/// </summary>
		[AdvancedInspector.Descriptor(" 据点列表(已存在的据点更新，新据点增加)", " 据点列表(已存在的据点更新，新据点增加)")]
		public TeamCopyFeild[] feildList = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)feildList.Length);
			for(int i = 0; i < feildList.Length; i++)
			{
				feildList[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 feildListCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref feildListCnt);
			feildList = new TeamCopyFeild[feildListCnt];
			for(int i = 0; i < feildList.Length; i++)
			{
				feildList[i] = new TeamCopyFeild();
				feildList[i].decode(buffer, ref pos_);
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
