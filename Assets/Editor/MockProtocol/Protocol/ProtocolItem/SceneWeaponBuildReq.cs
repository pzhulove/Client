using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  请求打造武器
	/// </summary>
	[AdvancedInspector.Descriptor(" 请求打造武器", " 请求打造武器")]
	public class SceneWeaponBuildReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501100;
		public UInt32 Sequence;

		public UInt32 id;

		public byte extraMaterial1;

		public byte extraMaterial2;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, id);
			BaseDLL.encode_int8(buffer, ref pos_, extraMaterial1);
			BaseDLL.encode_int8(buffer, ref pos_, extraMaterial2);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref id);
			BaseDLL.decode_int8(buffer, ref pos_, ref extraMaterial1);
			BaseDLL.decode_int8(buffer, ref pos_, ref extraMaterial2);
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
