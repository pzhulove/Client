using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  返回对战记录
	/// </summary>
	[AdvancedInspector.Descriptor(" 返回对战记录", " 返回对战记录")]
	public class SceneReplayListRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 507502;
		public UInt32 Sequence;
		/// <summary>
		///  录像列表类型（对应枚举ReplayListType）
		/// </summary>
		[AdvancedInspector.Descriptor(" 录像列表类型（对应枚举ReplayListType）", " 录像列表类型（对应枚举ReplayListType）")]
		public byte type;
		/// <summary>
		///  所有录像简介
		/// </summary>
		[AdvancedInspector.Descriptor(" 所有录像简介", " 所有录像简介")]
		public ReplayInfo[] replays = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, type);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)replays.Length);
			for(int i = 0; i < replays.Length; i++)
			{
				replays[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref type);
			UInt16 replaysCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref replaysCnt);
			replays = new ReplayInfo[replaysCnt];
			for(int i = 0; i < replays.Length; i++)
			{
				replays[i] = new ReplayInfo();
				replays[i].decode(buffer, ref pos_);
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
