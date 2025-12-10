using System;
using System.Collections.Generic;

[System.Serializable]
public class User
{
    public string name;
    public int level;
    public List<Session> history; // ← Maintenant List est reconnu
    
    public User(string playerName)
    {
        name = playerName;
        level = 1;
        history = new List<Session>();
    }
}