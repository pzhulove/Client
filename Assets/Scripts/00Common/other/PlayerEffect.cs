using UnityEngine;
using System.Collections;

public class PlayerEffect : MonoBehaviour
{
    public Player Player;

    [Header("原地攻击")]
    public GameObject SingleLeft;
    public GameObject DoubleLeft;
    public GameObject DoubleRight;

    public GameObject SingleLeftBack;
    public GameObject DoubleLeftBack;
    public GameObject DoubleRightBack;

    void SingleLeftFire()
    {
        GameObject go = Instantiate(Player.IsForward ? SingleLeft : SingleLeftBack, Player.transform.position, Player.transform.rotation) as GameObject;
        go.transform.parent = Player.transform;
    }

    void DoubleLeftFire()
    {
        GameObject go = Instantiate(Player.IsForward ? DoubleLeft : DoubleLeftBack, Player.transform.position, Player.transform.rotation) as GameObject;
        go.transform.parent = Player.transform;
    }

    void DoubleRightFire()
    {
        GameObject go = Instantiate(Player.IsForward ? DoubleRight : DoubleRightBack, Player.transform.position, Player.transform.rotation) as GameObject;
        go.transform.parent = Player.transform;
    }

    [Header("BUFF")]
    public GameObject Buff;
    public GameObject BuffBack;

    void AddBuff()
    {
        GameObject go = Instantiate(Player.IsForward ? Buff : BuffBack, Player.transform.position, Player.transform.rotation) as GameObject;
        go.transform.parent = Player.transform;
    }

    [Header("铲地")]
    public GameObject Dust;
    public GameObject DustBack;

    void AddDust()
    {
        GameObject go = Instantiate(Player.IsForward ? Dust : DustBack, Player.transform.position, Player.transform.rotation) as GameObject;
        go.transform.parent = Player.transform;
    }

    [Header("对地")]
    public GameObject DownAttack;
    public GameObject DownAttackRemain;
    public GameObject DownAttackBack;
    public GameObject DownAttackRemainBack;

    void GunFireDown()
    {
        GameObject go = Instantiate(Player.IsForward ? DownAttack : DownAttackBack, Player.transform.position, Player.transform.rotation) as GameObject;
        go.transform.parent = Player.transform;
        Instantiate(Player.IsForward ? DownAttackRemain : DownAttackRemainBack, Player.transform.position, Player.transform.rotation);
    }

    [Header("机枪")]
    public GameObject FireForward;
    public GameObject FireUp;
    public GameObject FireForwardBack;
    public GameObject FireUpBack;

    void GunFireForward()
    {
        GameObject go = Instantiate(Player.IsForward ? FireForward : FireForwardBack, Player.transform.position, Player.transform.rotation) as GameObject;
        go.transform.parent = Player.transform;
    }

    void GunFireUp()
    {
        GameObject go = Instantiate(Player.IsForward ? FireUp : FireUpBack, Player.transform.position, Player.transform.rotation) as GameObject;
        go.transform.parent = Player.transform;
    }

    [Header("移动射击")]
    public GameObject Walk_Left;
    public GameObject Walk_Right;
    public GameObject Walk_LeftBack;
    public GameObject Walk_RightBack;

    void GunWalkLeft()
    {
        GameObject go = Instantiate(Player.IsForward ? Walk_Left : Walk_LeftBack, Player.transform.position, Player.transform.rotation) as GameObject;
        go.transform.parent = Player.transform;
    }

    void GunWalkRight()
    {
        GameObject go = Instantiate(Player.IsForward ? Walk_Right : Walk_RightBack, Player.transform.position, Player.transform.rotation) as GameObject;
        go.transform.parent = Player.transform;
    }

    [Header("乱射")]
    public GameObject Luanshe;
    public GameObject LuansheBack;

    void GunLuanshe()
    {
        GameObject go = Instantiate(Player.IsForward ? Luanshe : LuansheBack, Player.transform.position, Player.transform.rotation) as GameObject;
        go.transform.parent = Player.transform;
    }

    [Header("踏射")]
    public GameObject Tashe;
    public GameObject TasheBack;

    void GunTashe()
    {
        GameObject go = Instantiate(Player.IsForward ? Tashe : TasheBack, Player.transform.position, Player.transform.rotation) as GameObject;
        go.transform.parent = Player.transform;
    }

    [Header("回旋踢")]
    public GameObject Huixuanti;
    public GameObject HuixuantiBack;

    void GunHuixuanti()
    {
        GameObject go = Instantiate(Player.IsForward ? Huixuanti : HuixuantiBack, Player.transform.position, Player.transform.rotation) as GameObject;
        go.transform.parent = Player.transform;
    }
}