using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  ˢ��������ʱ�䷵��
	/// </summary>
	[AdvancedInspector.Descriptor(" ˢ��������ʱ�䷵��", " ˢ��������ʱ�䷵��")]
	public class SceneAuctionRefreshRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 503902;
		public UInt32 Sequence;
		/// <summary>
		///  ���
		/// </summary>
		[AdvancedInspector.Descriptor(" ���", " ���")]
		public UInt32 result;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, result);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref result);
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
