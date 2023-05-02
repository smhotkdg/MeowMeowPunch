using UnityEngine;

namespace DungeonMaker.Demo
{
	[ExecuteInEditMode]
	public class SpelunkyDungeon : MonoBehaviour
	{
		public Generator generator;

		private void OnEnable() => Generator.OnGeneratorFinish += Check;
		private void OnDisable() => Generator.OnGeneratorFinish -= Check;

		private void Check(DungeonObject d)
		{
			RoomObject spawn = d.Rooms.Find(i => i.Name == "Spawn");
			RoomObject goal = d.Rooms.Find(i => i.Name == "Goal");

			if (spawn.Y < goal.Y)
			{
				generator.Destroy();
				generator.Generate();
			}
		}
	}
}