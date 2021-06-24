using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPath : MonoBehaviour
{
    public List<bool> rightJump100;
    public List<bool> rightJump80;
    public List<bool> rightJump60;
    public List<bool> rightJump40;
    public List<bool> rightDoubleJump1;
    public List<bool> rightDoubleJump2;
    public List<bool> rightDoubleJump3;
    public List<bool> rightDoubleJump4;

    public List<bool> leftJump100;
    public List<bool> leftJump80;
    public List<bool> leftJump60;
    public List<bool> leftJump40;
    public List<bool> leftDoubleJump1;
    public List<bool> leftDoubleJump2;
    public List<bool> leftDoubleJump3;
    public List<bool> leftDoubleJump4;

    public Transform RightJump100;
    public Transform RightJump80;
    public Transform RightJump60;
    public Transform RightJump40;
    public Transform RightDoubleJump1;
    public Transform RightDoubleJump2;
    public Transform RightDoubleJump3;
    public Transform RightDoubleJump4;

    public Transform LeftJump100;
    public Transform LeftJump80;
    public Transform LeftJump60;
    public Transform LeftJump40;
    public Transform LeftDoubleJump1;
    public Transform LeftDoubleJump2;
    public Transform LeftDoubleJump3;
    public Transform LeftDoubleJump4;

    void Awake()
    {
        //if the list lengths are updated, reuse this code to update them

        /*rightJump100 = new List<bool>(new bool[RightJump100.childCount]);
        rightJump80 = new List<bool>(new bool[RightJump80.childCount]);
        rightJump60 = new List<bool>(new bool[RightJump60.childCount]);
        rightJump40 = new List<bool>(new bool[RightJump40.childCount]);
        rightDoubleJump1 = new List<bool>(new bool[RightDoubleJump1.childCount]);
        rightDoubleJump2 = new List<bool>(new bool[RightDoubleJump2.childCount]);
        rightDoubleJump3 = new List<bool>(new bool[RightDoubleJump3.childCount]);
        rightDoubleJump4 = new List<bool>(new bool[RightDoubleJump4.childCount]);

        leftJump100 = new List<bool>(new bool[LeftJump100.childCount]);
        leftJump80 = new List<bool>(new bool[LeftJump80.childCount]);
        leftJump60 = new List<bool>(new bool[LeftJump60.childCount]);
        leftJump40 = new List<bool>(new bool[LeftJump40.childCount]);
        leftDoubleJump1 = new List<bool>(new bool[LeftDoubleJump1.childCount]);
        leftDoubleJump2 = new List<bool>(new bool[LeftDoubleJump2.childCount]);
        leftDoubleJump3 = new List<bool>(new bool[LeftDoubleJump3.childCount]);
        leftDoubleJump4 = new List<bool>(new bool[LeftDoubleJump4.childCount]);*/
    }
}
