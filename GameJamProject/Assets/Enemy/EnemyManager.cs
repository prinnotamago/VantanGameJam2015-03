﻿using UnityEngine;
using System.Collections;

public class EnemyManager : MonoBehaviour {

  /// <summary>
  /// ステージに出現する敵の数
  /// </summary>
  [SerializeField]
  private int spawnCount = 1;

  /// <summary>
  /// 一度に出現できる敵の最大数
  /// </summary>
  [SerializeField]
  private int spawnLimit = 10;

  /// <summary>
  /// ボスが出現するステージの倍数 ※バランス調整用
  /// </summary>
  [SerializeField]
  private int bossStageRatio = 5;

  /// <summary>
  /// ボス戦の制限時間
  /// </summary>
  [SerializeField]
  private float bossEncounterLimit = 30.0f;

  /// <summary>
  /// 現在表示されている敵の数
  /// </summary>
  private int liveEnemy = 0;

  /// <summary>
  /// ボス戦の残り時間
  /// </summary>
  private float encounterTime = 0.0f;

  /// <summary>
  /// ボスと戦闘中か
  /// </summary>
  private bool isBossBattle = false;


  void Start()
  {
    if (bossStageRatio <= 1) bossStageRatio = 2;
    if (spawnLimit < 1) spawnLimit = 1;
    if (liveEnemy > 0) liveEnemy = 0;
  }

  void Update()
  {
    if (encounterTime > 0.0f) encounterTime -= Time.deltaTime;
    if (IsTimeOver()) isBossBattle = false;
  }
  
  private bool BossStage()
  {
    var stage = FindObjectOfType(typeof(StageInformation)) as StageInformation;
    if (stage.nowStage <= 1) return false;

    return (stage.nowStage % bossStageRatio) == 0;
  }

  // StageManager
  /// <summary>
  /// 敵が全滅したら true を返す
  /// </summary>
  public bool IsEnemyExtinction()
  {
    if (!BossStage()) return spawnCount <= 0;

    var boss = FindObjectOfType(typeof(EnemyLifeBoss)) as EnemyLifeBoss;
    return boss.IsDead();
  }

  // StageManager
  /// <summary>
  /// 出現する敵の数を指定
  /// </summary>
  public void SetSpawnCount()
  {
    var stage = FindObjectOfType(typeof(StageInformation)) as StageInformation;
    spawnCount = (int)stage.StageMobNumber();
  }

  // UI
  /// <summary>
  /// ボス戦モードに切り替える
  /// </summary>
  public void BossSpawnSwitch()
  {
    if (encounterTime > 0.0f && !BossStage()) return;

    encounterTime = bossEncounterLimit;
    isBossBattle = true;

    var spawn = FindObjectOfType(typeof(EnemySpawnBoss)) as EnemySpawnBoss;
    spawn.SetBossData();
  }
  
  // EnemySpawnMob.sc
  /// <summary>
  /// モブが出現できるとき true を返す
  /// </summary>
  public bool IsAbleToSpawnMob()
  {
    return (!isBossBattle && liveEnemy < spawnLimit && spawnCount > 0);
  }

  // EnemySpawnMob.sc
  /// <summary>
  /// 表示しているモブの数を増やしてモブ残数を減らす
  /// </summary>
  public void MobSpawn()
  {
    --spawnCount;
    ++liveEnemy;
  }
  
  // EnemyLifeMob.sc, EnemyLifeBoss.sc
  /// <summary>
  /// 表示しているモブの数を減らす
  /// </summary>
  public void EnemyDead()
  {
    --liveEnemy;
  }
  
  // EnemyLifeBoss.sc
  /// <summary>
  /// 時間内にボスを倒せなかったら true を返す
  /// </summary>
  public bool IsTimeOver()
  {
    if (!BossStage()) return false;

    var boss = FindObjectOfType(typeof(EnemyLifeBoss)) as EnemyLifeBoss;
    return (encounterTime <= 0.0f && !boss.IsDead());
  }
}
