using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  设置武器栏请求
	/// </summary>
	[AdvancedInspector.Descriptor(" 设置武器栏请求", " 设置武器栏请求")]
	public class SceneSetWeaponBarReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501219;
		public UInt32 Sequence;

		public byte index;

		public UInt64 weaponId;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, index);
			BaseDLL.encode_uint64(buffer, ref pos_, weaponId);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref index);
			BaseDLL.decode_uint64(buffer, ref pos_, ref weaponId);
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
