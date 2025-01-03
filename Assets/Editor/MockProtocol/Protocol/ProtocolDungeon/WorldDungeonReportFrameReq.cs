using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  玩家上报帧数据
	/// </summary>
	[AdvancedInspector.Descriptor(" 玩家上报帧数据", " 玩家上报帧数据")]
	public class WorldDungeonReportFrameReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 606811;
		public UInt32 Sequence;
		/// <summary>
		///  操作帧
		/// </summary>
		[AdvancedInspector.Descriptor(" 操作帧", " 操作帧")]
		public Frame[] frames = null;
		/// <summary>
		///  随机数
		/// </summary>
		[AdvancedInspector.Descriptor(" 随机数", " 随机数")]
		public FrameChecksum[] checksums = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)frames.Length);
			for(int i = 0; i < frames.Length; i++)
			{
				frames[i].encode(buffer, ref pos_);
			}
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)checksums.Length);
			for(int i = 0; i < checksums.Length; i++)
			{
				checksums[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 framesCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref framesCnt);
			frames = new Frame[framesCnt];
			for(int i = 0; i < frames.Length; i++)
			{
				frames[i] = new Frame();
				frames[i].decode(buffer, ref pos_);
			}
			UInt16 checksumsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref checksumsCnt);
			checksums = new FrameChecksum[checksumsCnt];
			for(int i = 0; i < checksums.Length; i++)
			{
				checksums[i] = new FrameChecksum();
				checksums[i].decode(buffer, ref pos_);
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
