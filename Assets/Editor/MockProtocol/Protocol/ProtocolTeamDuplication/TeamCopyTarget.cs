using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  目标
	/// </summary>
	[AdvancedInspector.Descriptor(" 目标", " 目标")]
	public class TeamCopyTarget : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  目标id
		/// </summary>
		[AdvancedInspector.Descriptor(" 目标id", " 目标id")]
		public UInt32 targetId;
		/// <summary>
		///  目标详情
		/// </summary>
		[AdvancedInspector.Descriptor(" 目标详情", " 目标详情")]
		public TeamCopyTargetDetail[] targetDetailList = null;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, targetId);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)targetDetailList.Length);
			for(int i = 0; i < targetDetailList.Length; i++)
			{
				targetDetailList[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref targetId);
			UInt16 targetDetailListCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref targetDetailListCnt);
			targetDetailList = new TeamCopyTargetDetail[targetDetailListCnt];
			for(int i = 0; i < targetDetailList.Length; i++)
			{
				targetDetailList[i] = new TeamCopyTargetDetail();
				targetDetailList[i].decode(buffer, ref pos_);
			}
		}


		#endregion

	}

}
