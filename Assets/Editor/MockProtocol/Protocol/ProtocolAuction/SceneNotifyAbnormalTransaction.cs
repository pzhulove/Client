using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// scene->client ֪ͨ�쳣����
	/// </summary>
	[AdvancedInspector.Descriptor("scene->client ֪ͨ�쳣����", "scene->client ֪ͨ�쳣����")]
	public class SceneNotifyAbnormalTransaction : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 503905;
		public UInt32 Sequence;
		/// <summary>
		///  boolֵ(false(0):�ر�֪ͨ��true(1):����֪ͨ)
		/// </summary>
		[AdvancedInspector.Descriptor(" boolֵ(false(0):�ر�֪ͨ��true(1):����֪ͨ)", " boolֵ(false(0):�ر�֪ͨ��true(1):����֪ͨ)")]
		public byte bNotify;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, bNotify);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref bNotify);
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
