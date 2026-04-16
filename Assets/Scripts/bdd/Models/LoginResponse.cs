using System;
using System.Collections.Generic;

[Serializable]
public class LoginResponse
{
    public int id;
    public string username;
    public List<LevelResult> stats;
}