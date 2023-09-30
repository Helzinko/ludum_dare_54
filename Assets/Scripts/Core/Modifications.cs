using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IModification
{
    void ApplyModification();

    void Init();
    string Title();
    string Description();
    ModificationSO[] UnlockedModifications();
}

public class BaseModification : IModification
{
    public ModificationSO[] unlockedModifications;

    public virtual void ApplyModification()
    {
        throw new System.NotImplementedException();
    }

    public virtual string Description()
    {
        throw new System.NotImplementedException();
    }

    public virtual void Init()
    {

    }

    public virtual void ModifyEnemy(Enemy enemy)
    {
        throw new NotImplementedException();
    }

    public virtual string Title()
    {
        throw new System.NotImplementedException();
    }

    ModificationSO[] IModification.UnlockedModifications()
    {
        return unlockedModifications;
    }
}

public class AddEnemy : BaseModification
{
    public GameEntity gameEntity;

    public override void ApplyModification()
    {
        GameController.instance.AddEnemy(gameEntity as Enemy);
    }

    public override string Title()
    {
        return "+<color=#e43b44>" + (gameEntity as Enemy).Name + "</color>";
    }

    public override string Description()
    {
        return "<color=#e43b44>" + (gameEntity as Enemy).Name + "</color> " + (gameEntity as Enemy).Description;
    }
}

public class EnemyModification : BaseModification
{
    [SerializeField] private string description;

    protected Enemy enemyToModify;

    public override string Title()
    {
        return "+<color=#e43b44>" + enemyToModify.Name + "</color>" + " modification";
    }

    public override string Description()
    {
        return description;
    }

    public T FindEnemy<T>() where T : Enemy
    {
        return GameObject.FindObjectOfType<T>();
    }
}


public class LaserEnemyModification : EnemyModification
{
    protected LaserEnemy laserEnemy => enemyToModify as LaserEnemy;

    public override void Init()
    {
        base.Init();
        enemyToModify = FindEnemy<LaserEnemy>();
    }
}

public class MissilesEnemyModification : EnemyModification
{
    protected MissileLauncher missilesEnemy => enemyToModify as MissileLauncher;

    public override void Init()
    {
        base.Init();
        enemyToModify = FindEnemy<MissileLauncher>();
    }
}

public class ModifyLaserRestTime : LaserEnemyModification
{
    [SerializeField] private float restTime;
    public override void ApplyModification()
    {
        laserEnemy.restTime = restTime;
    }
}

public class ModifyLaunchedMissilesCount : MissilesEnemyModification
{
    [SerializeField] private int launchedMissiles;
    public override void ApplyModification()
    {
        missilesEnemy.missilesToLaunch = launchedMissiles;
    }
}