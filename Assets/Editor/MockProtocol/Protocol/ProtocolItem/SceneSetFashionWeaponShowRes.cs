using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  scene->client 设置时装武器显示返回
	/// </summary>
	[AdvancedInspector.Descriptor(" scene->client 设置时装武器显示返回", " scene->client 设置时装武器显示返回")]
	public class SceneSetFashionWeaponShowRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 501028;
		public UInt32 Sequence;
		/// <summary>
		///  返回值
		/// </summary>
		[AdvancedInspector.Descriptor(" 返回值", " 返回值")]
		public UInt32 ret;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, ret);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref ret);
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
