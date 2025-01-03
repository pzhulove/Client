using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  �ͽ���������
	/// </summary>
	[AdvancedInspector.Descriptor(" �ͽ���������", " �ͽ���������")]
	public class WorldPremiumLeagueRaceEnd : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 607712;
		public UInt32 Sequence;
		/// <summary>
		///  �ǲ���Ԥѡ��
		/// </summary>
		[AdvancedInspector.Descriptor(" �ǲ���Ԥѡ��", " �ǲ���Ԥѡ��")]
		public byte isPreliminay;
		/// <summary>
		///  ս�����
		/// </summary>
		[AdvancedInspector.Descriptor(" ս�����", " ս�����")]
		public byte result;
		/// <summary>
		///  ԭ�л���
		/// </summary>
		[AdvancedInspector.Descriptor(" ԭ�л���", " ԭ�л���")]
		public UInt32 oldScore;
		/// <summary>
		///  �»���
		/// </summary>
		[AdvancedInspector.Descriptor(" �»���", " �»���")]
		public UInt32 newScore;
		/// <summary>
		///  ��������
		/// </summary>
		[AdvancedInspector.Descriptor(" ��������", " ��������")]
		public UInt32 preliminayRewardNum;
		/// <summary>
		///  �������
		/// </summary>
		[AdvancedInspector.Descriptor(" �������", " �������")]
		public UInt32 getHonor;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, isPreliminay);
			BaseDLL.encode_int8(buffer, ref pos_, result);
			BaseDLL.encode_uint32(buffer, ref pos_, oldScore);
			BaseDLL.encode_uint32(buffer, ref pos_, newScore);
			BaseDLL.encode_uint32(buffer, ref pos_, preliminayRewardNum);
			BaseDLL.encode_uint32(buffer, ref pos_, getHonor);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref isPreliminay);
			BaseDLL.decode_int8(buffer, ref pos_, ref result);
			BaseDLL.decode_uint32(buffer, ref pos_, ref oldScore);
			BaseDLL.decode_uint32(buffer, ref pos_, ref newScore);
			BaseDLL.decode_uint32(buffer, ref pos_, ref preliminayRewardNum);
			BaseDLL.decode_uint32(buffer, ref pos_, ref getHonor);
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
