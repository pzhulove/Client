using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  ������������λ����
	/// </summary>
	[AdvancedInspector.Descriptor(" ������������λ����", " ������������λ����")]
	public class SceneAuctionBuyBoothRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 503904;
		public UInt32 Sequence;
		/// <summary>
		///  ���
		/// </summary>
		[AdvancedInspector.Descriptor(" ���", " ���")]
		public UInt32 result;
		/// <summary>
		///  ��λ��
		/// </summary>
		[AdvancedInspector.Descriptor(" ��λ��", " ��λ��")]
		public UInt32 boothNum;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, result);
			BaseDLL.encode_uint32(buffer, ref pos_, boothNum);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			BaseDLL.decode_uint32(buffer, ref pos_, ref boothNum);
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
