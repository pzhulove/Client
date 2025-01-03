using System;
using System.Text;

namespace Mock.Protocol
{

	public class TeamCopyTargetDetail : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  据点id
		/// </summary>
		[AdvancedInspector.Descriptor(" 据点id", " 据点id")]
		public UInt32 fieldId;
		/// <summary>
		///  当前次数
		/// </summary>
		[AdvancedInspector.Descriptor(" 当前次数", " 当前次数")]
		public UInt32 curNum;
		/// <summary>
		///  总次数
		/// </summary>
		[AdvancedInspector.Descriptor(" 总次数", " 总次数")]
		public UInt32 totalNum;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, fieldId);
			BaseDLL.encode_uint32(buffer, ref pos_, curNum);
			BaseDLL.encode_uint32(buffer, ref pos_, totalNum);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref fieldId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref curNum);
			BaseDLL.decode_uint32(buffer, ref pos_, ref totalNum);
		}


		#endregion

	}

}
