using UnityEngine;
using System.Collections;

public class ControlDoorState : MonoBehaviour {

	// Use this for initialization
	public GameObject CloseDoorObj;
	public GameObject OpenDoorObj;

	public GameObject OpenDoorObj_TOP;
	public GameObject OpenDoorObj_BOTTOM;
	public GameObject OpenDoorObj_LEFT;
	public GameObject OpenDoorObj_RIGHT;

	private GameObject[] doorsEffect;

	void Awake () {

		if (doorsEffect == null)
		{
			doorsEffect = new GameObject[]{OpenDoorObj_LEFT, OpenDoorObj_TOP, OpenDoorObj_RIGHT,OpenDoorObj_BOTTOM};
		}

        CloseDoor ();
	}

	public void OpenDoor(TransportDoorType doorType = TransportDoorType.None)
	{
		if(CloseDoorObj)
		{
			if (CloseDoorObj != null)
				CloseDoorObj.CustomActive(false);
			if (OpenDoorObj != null)
				OpenDoorObj.CustomActive(true);
			SetOpenDoor(doorType);
		}
	}
	public void CloseDoor()
	{
		//if(CloseDoorObj && OpenDoorObj)
		{
			if (CloseDoorObj != null)
				CloseDoorObj.CustomActive(true);
			if (OpenDoorObj != null)
				OpenDoorObj.CustomActive(false);
			SetOpenDoorVisible(false);
        }
	}

	private void SetOpenDoorVisible(bool flag)
	{
		for(int i=0; i<doorsEffect.Length; ++i)
			if (doorsEffect[i] != null)
				doorsEffect[i].CustomActive(flag);
	}

	private void SetOpenDoor(TransportDoorType doorType)
	{
		if (doorType >= TransportDoorType.None)
			return;
		
		if (doorsEffect[(int)doorType] != null)
			doorsEffect[(int)doorType].CustomActive(true);
	}

	public Vector3 GetRegionPos(TransportDoorType doorType)
	{
		if (doorsEffect[(int)doorType] != null)
			return doorsEffect[(int)doorType].transform.position;

		return Vector3.zero;
	}
}
