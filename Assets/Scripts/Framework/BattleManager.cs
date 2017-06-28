using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BattleManager : PersistentSingleton<BattleManager> 
{
	public AudioClip battleMusic;
	public BattleTurn battleTurn;
	public List<Enemy> enemies = new List<Enemy>();
	
	public void ClearBattleRoster()
	{
		
	}

	public void InitializeBattle(Enemy[] newEnemies)
	{
		foreach (Enemy enemy in newEnemies)
			enemies.Add(new Enemy(enemy));
		RoomManager.instance.ChangeScene (OverworldRoomData.instance.roomBattleScene, RoomLoadType.Battle);
	}

	public void SetupBattleScene()
	{
		switch (enemies.Count)
		{
		case 1:
			enemies [0].SpawnEnemyModelInstance (BattleRoomData.instance.enemyPositions [0].position, BattleRoomData.instance.enemyPositions [0].rotation);
			break;
		case 2:
			enemies [0].SpawnEnemyModelInstance (BattleRoomData.instance.enemyPositions [1].position, BattleRoomData.instance.enemyPositions [1].rotation);
			enemies [1].SpawnEnemyModelInstance (BattleRoomData.instance.enemyPositions [2].position, BattleRoomData.instance.enemyPositions [2].rotation);
			break;
		case 3:
			enemies [0].SpawnEnemyModelInstance (BattleRoomData.instance.enemyPositions [0].position, BattleRoomData.instance.enemyPositions [0].rotation);
			enemies [1].SpawnEnemyModelInstance (BattleRoomData.instance.enemyPositions [1].position, BattleRoomData.instance.enemyPositions [1].rotation);
			enemies [2].SpawnEnemyModelInstance (BattleRoomData.instance.enemyPositions [2].position, BattleRoomData.instance.enemyPositions [2].rotation);
			break;
		case 4:
			enemies [0].SpawnEnemyModelInstance (BattleRoomData.instance.enemyPositions [1].position, BattleRoomData.instance.enemyPositions [1].rotation);
			enemies [1].SpawnEnemyModelInstance (BattleRoomData.instance.enemyPositions [2].position, BattleRoomData.instance.enemyPositions [2].rotation);
			enemies [2].SpawnEnemyModelInstance (BattleRoomData.instance.enemyPositions [3].position, BattleRoomData.instance.enemyPositions [3].rotation);
			enemies [3].SpawnEnemyModelInstance (BattleRoomData.instance.enemyPositions [4].position, BattleRoomData.instance.enemyPositions [4].rotation);
			break;
		}
	}
}

public enum BattleTurn
{
	Player,
	Enemy,
}
