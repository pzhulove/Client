using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  掉落的buff信息
	/// </summary>
	[AdvancedInspector.Descriptor(" 掉落的buff信息", " 掉落的buff信息")]
	public class GuildDungeonBuff : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{
		/// <summary>
		///  id
		/// </summary>
		[AdvancedInspector.Descriptor(" id", " id")]
		public UInt32 buffId;
		/// <summary>
		///  等级
		/// </summary>
		[AdvancedInspector.Descriptor(" 等级", " 等级")]
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
