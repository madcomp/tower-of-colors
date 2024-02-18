using System.Collections;
using System.Collections.Generic;

public class Player
{
    static Player _instance;
    public static Player Instance
    {
        get {
            if (_instance == null)
            {
                _instance = new Player();
            }
            return _instance;
        }
    }
    
    public Wallet Wallet { get; }

    Player()
    {
        Wallet = new Wallet();
    }
}
