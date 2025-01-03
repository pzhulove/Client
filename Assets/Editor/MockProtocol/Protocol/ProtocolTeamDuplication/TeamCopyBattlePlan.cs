using System;
using System.Text;

namespace Mock.Protocol
{

	public class TeamCopyBattlePlan : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  难度
		/// </summary>
		[AdvancedInspector.Descriptor(" 难度", " 难度")]
		public UInt32 difficulty;
		/// <summary>
		///  小队ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 小队ID", " 小队ID")]
		public UInt32 squadId;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, difficulty);
			BaseDLL.encode_uint32(buffer, ref pos_, squadId);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref difficulty);
			BaseDLL.decode_uint32(buffer, ref pos_, ref squadId);
		}


		#endregion

	}

}
