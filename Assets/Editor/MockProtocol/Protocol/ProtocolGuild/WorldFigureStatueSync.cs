using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  雕像同步
	/// </summary>
	[AdvancedInspector.Descriptor(" 雕像同步", " 雕像同步")]
	public class WorldFigureStatueSync : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 600108;
		public UInt32 Sequence;

		public FigureStatueInfo[] figureStatue = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)figureStatue.Length);
			for(int i = 0; i < figureStatue.Length; i++)
			{
				figureStatue[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 figureStatueCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref figureStatueCnt);
			figureStatue = new FigureStatueInfo[figureStatueCnt];
			for(int i = 0; i < figureStatue.Length; i++)
			{
				figureStatue[i] = new FigureStatueInfo();
				figureStatue[i].decode(buffer, ref pos_);
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