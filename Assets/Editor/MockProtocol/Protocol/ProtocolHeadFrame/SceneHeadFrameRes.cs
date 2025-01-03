using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  头像框返回
	/// </summary>
	[AdvancedInspector.Descriptor(" 头像框返回", " 头像框返回")]
	public class SceneHeadFrameRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 509102;
		public UInt32 Sequence;
		/// <summary>
		///  头像框列表
		/// </summary>
		[AdvancedInspector.Descriptor(" 头像框列表", " 头像框列表")]
		public HeadFrame[] headFrameList = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)headFrameList.Length);
			for(int i = 0; i < headFrameList.Length; i++)
			{
				headFrameList[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 headFrameListCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref headFrameListCnt);
			headFrameList = new HeadFrame[headFrameListCnt];
			for(int i = 0; i < headFrameList.Length; i++)
			{
				headFrameList[i] = new HeadFrame();
				headFrameList[i].decode(buffer, ref pos_);
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
