using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  吃鸡开宝箱请求
	/// </summary>
	[AdvancedInspector.Descriptor(" 吃鸡开宝箱请求", " 吃鸡开宝箱请求")]
	public class SceneBattleOpenBoxReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 508935;
		public UInt32 Sequence;

		public UInt64 itemGuid;
		/// <summary>
		///  参数 1：打开；2：取消；3：拾取
		/// </summary>
		[AdvancedInspector.Descriptor(" 参数 1：打开；2：取消；3：拾取", " 参数 1：打开；2：取消；3：拾取")]
		public UInt32 param;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, itemGuid);
			BaseDLL.encode_uint32(buffer, ref pos_, param);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref itemGuid);
			BaseDLL.decode_uint32(buffer, ref pos_, ref param);
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
