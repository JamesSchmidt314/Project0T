using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleRoomData : Singleton<BattleRoomData> 
{
	public Transform[] playerPositions;
	public Transform[] enemyPositions;
}