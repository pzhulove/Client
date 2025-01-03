using System;
using System.Text;

namespace Mock.Protocol
{

	public class ScenePlayerChangeSceneReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 500503;
		public UInt32 Sequence;

		public UInt32 curDoorId;

		public UInt32 dstSceneId;

		public UInt32 dstDoorId;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, curDoorId);
			BaseDLL.encode_uint32(buffer, ref pos_, dstSceneId);
			BaseDLL.encode_uint32(buffer, ref pos_, dstDoorId);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref curDoorId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref dstSceneId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref dstDoorId);
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
