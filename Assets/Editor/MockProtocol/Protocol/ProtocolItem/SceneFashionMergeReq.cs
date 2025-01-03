using System;
using System.Text;

namespace Mock.Protocol
{

	public class SceneFashionMergeReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 500952;
		public UInt32 Sequence;

		public UInt64 leftid;
		/// <summary>
		///  时装A
		/// </summary>
		[AdvancedInspector.Descriptor(" 时装A", " 时装A")]
		public UInt64 rightid;
		/// <summary>
		///  时装B
		/// </summary>
		[AdvancedInspector.Descriptor(" 时装B", " 时装B")]
		public UInt64 composer;
		/// <summary>
		///  合成器
		/// </summary>
		[AdvancedInspector.Descriptor(" 合成器", " 合成器")]
		public UInt32 skySuitID;
		/// <summary>
		///  选择的天空套套装ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 选择的天空套套装ID", " 选择的天空套套装ID")]
		public UInt32 selFashionID;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, leftid);
			BaseDLL.encode_uint64(buffer, ref pos_, rightid);
			BaseDLL.encode_uint64(buffer, ref pos_, composer);
			BaseDLL.encode_uint32(buffer, ref pos_, skySuitID);
			BaseDLL.encode_uint32(buffer, ref pos_, selFashionID);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref leftid);
			BaseDLL.decode_uint64(buffer, ref pos_, ref rightid);
			BaseDLL.decode_uint64(buffer, ref pos_, ref composer);
			BaseDLL.decode_uint32(buffer, ref pos_, ref skySuitID);
			BaseDLL.decode_uint32(buffer, ref pos_, ref selFashionID);
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
