using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  神器罐子奖励配置
	/// </summary>
	[AdvancedInspector.Descriptor(" 神器罐子奖励配置", " 神器罐子奖励配置")]
	public class ArtifactJarLotteryCfg : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  罐子id
		/// </summary>
		[AdvancedInspector.Descriptor(" 罐子id", " 罐子id")]
		public UInt32 jarId;
		/// <summary>
		///  奖励列表
		/// </summary>
		[AdvancedInspector.Descriptor(" 奖励列表", " 奖励列表")]
		public ItemReward[] rewardList = null;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, jarId);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)rewardList.Length);
			for(int i = 0; i < rewardList.Length; i++)
			{
				rewardList[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref jarId);
			UInt16 rewardListCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref rewardListCnt);
			rewardList = new ItemReward[rewardListCnt];
			for(int i = 0; i < rewardList.Length; i++)
			{
				rewardList[i] = new ItemReward();
				rewardList[i].decode(buffer, ref pos_);
			}
		}


		#endregion

	}

}
