using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  修改组队buff药配置
	/// </summary>
	[AdvancedInspector.Descriptor(" 修改组队buff药配置", " 修改组队buff药配置")]
	public class WorldTeamModifyBufferDrugs : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601658;
		public UInt32 Sequence;
		/// <summary>
		///  buff药配置
		/// </summary>
		[AdvancedInspector.Descriptor(" buff药配置", " buff药配置")]
		public UInt32[] bufferDrugs = new UInt32[0];

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)bufferDrugs.Length);
			for(int i = 0; i < bufferDrugs.Length; i++)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, bufferDrugs[i]);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 bufferDrugsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref bufferDrugsCnt);
			bufferDrugs = new UInt32[bufferDrugsCnt];
			for(int i = 0; i < bufferDrugs.Length; i++)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref bufferDrugs[i]);
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
