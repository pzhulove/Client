using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  活动神器罐购买次数返回
	/// </summary>
	[AdvancedInspector.Descriptor(" 活动神器罐购买次数返回", " 活动神器罐购买次数返回")]
	public class SceneArtifactJarBuyCntRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501047;
		public UInt32 Sequence;

		public ArtifactJarBuy[] artifactJarBuyList = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)artifactJarBuyList.Length);
			for(int i = 0; i < artifactJarBuyList.Length; i++)
			{
				artifactJarBuyList[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 artifactJarBuyListCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref artifactJarBuyListCnt);
			artifactJarBuyList = new ArtifactJarBuy[artifactJarBuyListCnt];
			for(int i = 0; i < artifactJarBuyList.Length; i++)
			{
				artifactJarBuyList[i] = new ArtifactJarBuy();
				artifactJarBuyList[i].decode(buffer, ref pos_);
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
