using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  神器罐活动抽奖记录返回
	/// </summary>
	[AdvancedInspector.Descriptor(" 神器罐活动抽奖记录返回", " 神器罐活动抽奖记录返回")]
	public class GASArtifactJarLotteryCfgRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 700906;
		public UInt32 Sequence;

		public ArtifactJarLotteryCfg[] artifactJarCfgList = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)artifactJarCfgList.Length);
			for(int i = 0; i < artifactJarCfgList.Length; i++)
			{
				artifactJarCfgList[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 artifactJarCfgListCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref artifactJarCfgListCnt);
			artifactJarCfgList = new ArtifactJarLotteryCfg[artifactJarCfgListCnt];
			for(int i = 0; i < artifactJarCfgList.Length; i++)
			{
				artifactJarCfgList[i] = new ArtifactJarLotteryCfg();
				artifactJarCfgList[i].decode(buffer, ref pos_);
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
