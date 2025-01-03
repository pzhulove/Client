using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  ��������Ϣ
	/// </summary>
	[AdvancedInspector.Descriptor(" ��������Ϣ", " ��������Ϣ")]
	public class GuildRedPacketSpecInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  �������
		/// </summary>
		[AdvancedInspector.Descriptor(" �������", " �������")]
		public byte type;
		/// <summary>
		///  ʱ��
		/// </summary>
		[AdvancedInspector.Descriptor(" ʱ��", " ʱ��")]
		public UInt32 lastTime;
		/// <summary>
		///  ��������
		/// </summary>
		[AdvancedInspector.Descriptor(" ��������", " ��������")]
		public UInt32 joinNum;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, type);
			BaseDLL.encode_uint32(buffer, ref pos_, lastTime);
			BaseDLL.encode_uint32(buffer, ref pos_, joinNum);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref type);
			BaseDLL.decode_uint32(buffer, ref pos_, ref lastTime);
			BaseDLL.decode_uint32(buffer, ref pos_, ref joinNum);
		}


		#endregion

	}

}
