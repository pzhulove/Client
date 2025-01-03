using System;
using System.Text;

namespace Mock.Protocol
{

	public class DungeonBuff : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  buffid
		/// </summary>
		[AdvancedInspector.Descriptor(" buffid", " buffid")]
		public UInt32 buffId;
		/// <summary>
		///  buff等级
		/// </summary>
		[AdvancedInspector.Descriptor(" buff等级", " buff等级")]
		public UInt32 buffLvl;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, buffId);
			BaseDLL.encode_uint32(buffer, ref pos_, buffLvl);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref buffId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref buffLvl);
		}


		#endregion

	}

}
