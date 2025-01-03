using System;
using System.Text;

namespace Mock.Protocol
{

	public class PlayerAvatar : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt32[] equipItemIds = new UInt32[0];
		/// <summary>
		///  武器强化等级
		/// </summary>
		[AdvancedInspector.Descriptor(" 武器强化等级", " 武器强化等级")]
		public byte weaponStrengthen;
		/// <summary>
		///  是否显示时装武器
		/// </summary>
		[AdvancedInspector.Descriptor(" 是否显示时装武器", " 是否显示时装武器")]
		public byte isShoWeapon;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)equipItemIds.Length);
			for(int i = 0; i < equipItemIds.Length; i++)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, equipItemIds[i]);
			}
			BaseDLL.encode_int8(buffer, ref pos_, weaponStrengthen);
			BaseDLL.encode_int8(buffer, ref pos_, isShoWeapon);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 equipItemIdsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref equipItemIdsCnt);
			equipItemIds = new UInt32[equipItemIdsCnt];
			for(int i = 0; i < equipItemIds.Length; i++)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref equipItemIds[i]);
			}
			BaseDLL.decode_int8(buffer, ref pos_, ref weaponStrengthen);
			BaseDLL.decode_int8(buffer, ref pos_, ref isShoWeapon);
		}


		#endregion

	}

}
