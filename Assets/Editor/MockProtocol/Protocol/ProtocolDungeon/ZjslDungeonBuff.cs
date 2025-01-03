using System;
using System.Text;

namespace Mock.Protocol
{

	public class ZjslDungeonBuff : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
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
		/// <summary>
		///  buff对象，1：玩家；2：怪物
		/// </summary>
		[AdvancedInspector.Descriptor(" buff对象，1：玩家；2：怪物", " buff对象，1：玩家；2：怪物")]
		public UInt32 buffTarget;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, buffId);
			BaseDLL.encode_uint32(buffer, ref pos_, buffLvl);
			BaseDLL.encode_uint32(buffer, ref pos_, buffTarget);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref buffId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref buffLvl);
			BaseDLL.decode_uint32(buffer, ref pos_, ref buffTarget);
		}


		#endregion

	}

}
