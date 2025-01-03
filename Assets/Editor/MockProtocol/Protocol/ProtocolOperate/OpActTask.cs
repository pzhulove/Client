using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 运营活动任务数据
	/// </summary>
	[AdvancedInspector.Descriptor("运营活动任务数据", "运营活动任务数据")]
	public class OpActTask : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt32 dataId;
		/// <summary>
		/// id
		/// </summary>
		[AdvancedInspector.Descriptor("id", "id")]
		public UInt32 curNum;
		/// <summary>
		/// 当前数量
		/// </summary>
		[AdvancedInspector.Descriptor("当前数量", "当前数量")]
		public byte state;
		/// <summary>
		/// 状态OpActTaskState
		/// </summary>
		[AdvancedInspector.Descriptor("状态OpActTaskState", "状态OpActTaskState")]
		public OpActTaskParam[] parms = null;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, dataId);
			BaseDLL.encode_uint32(buffer, ref pos_, curNum);
			BaseDLL.encode_int8(buffer, ref pos_, state);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)parms.Length);
			for(int i = 0; i < parms.Length; i++)
			{
				parms[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref dataId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref curNum);
			BaseDLL.decode_int8(buffer, ref pos_, ref state);
			UInt16 parmsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref parmsCnt);
			parms = new OpActTaskParam[parmsCnt];
			for(int i = 0; i < parms.Length; i++)
			{
				parms[i] = new OpActTaskParam();
				parms[i].decode(buffer, ref pos_);
			}
		}


		#endregion

	}

}
