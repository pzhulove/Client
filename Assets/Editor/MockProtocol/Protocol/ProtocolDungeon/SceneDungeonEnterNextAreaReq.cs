using System;
using System.Text;

namespace Mock.Protocol
{

	public class SceneDungeonEnterNextAreaReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 506807;
		public UInt32 Sequence;

		public UInt32 areaId;

		public UInt32 lastFrame;

		public DungeonStaticValue staticVal = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, areaId);
			BaseDLL.encode_uint32(buffer, ref pos_, lastFrame);
			staticVal.encode(buffer, ref pos_);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref areaId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref lastFrame);
			staticVal.decode(buffer, ref pos_);
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
