using System;
using System.Text;

namespace Mock.Protocol
{

	public class SceneMoveRequire : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 500501;
		public UInt32 Sequence;
		/// <summary>
		///  客户端所在场景
		/// </summary>
		[AdvancedInspector.Descriptor(" 客户端所在场景", " 客户端所在场景")]
		public UInt32 clientSceneId;

		public ScenePosition pos = null;

		public SceneDir dir = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, clientSceneId);
			pos.encode(buffer, ref pos_);
			dir.encode(buffer, ref pos_);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref clientSceneId);
			pos.decode(buffer, ref pos_);
			dir.decode(buffer, ref pos_);
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
