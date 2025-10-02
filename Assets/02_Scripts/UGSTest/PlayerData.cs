using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PlayerStat
{
    public int statPoint;
    public int strength;
    public int dexterity;
    public int intelligence;
    public int vitality;
}

[System.Serializable]
public class PlayerSkill
{
    public int skillPoint;
    public List<string> unlockedSkillID;
    public List<string> lockedSkillID;
}

[System.Serializable]
public class PlayerData
{
    public string name;

    public int level;
    public float curEXP;
    public float curHP;
    public float curMP;

    public float moveSpeed;

    public Vector3 lastPos;

    public PlayerStat stat;
    public PlayerSkill skill;

    public int HPPotionCount;
    public int MPPotionCount;
    
    public Dictionary<string, string> equippedItems;
    public Dictionary<string, int> questProgress;

    public List<string> equippedSlotKeys;
    public List<string> equippedItemValues;
    public List<string> questKeys;
    public List<int> questValues;

    public PlayerData()
    {
        name = "Player";
        level = 1;
        curEXP = 0f;
        curHP = 100f;
        curMP = 100f;

        moveSpeed = 5f;

        lastPos = Vector3.zero;

        stat = new PlayerStat { strength = 5, dexterity = 5, intelligence = 5, vitality = 5 };
        skill = new PlayerSkill { skillPoint = 1, unlockedSkillID = new List<string>(), lockedSkillID = new List<string> { "Skill01_Lv1", "Skill02_Lv1" } };

        HPPotionCount = 3;
        MPPotionCount = 3;

        equippedItems = new Dictionary<string, string>();
        questProgress = new Dictionary<string, int>();

        equippedSlotKeys = new List<string>();
        equippedItemValues = new List<string>();
        questKeys = new List<string>();
        questValues = new List<int>();
    }
}
