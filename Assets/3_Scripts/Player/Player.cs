using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public Wallet Wallet { get; }

    public Player()
    {
        Wallet = new Wallet();
    }
}
