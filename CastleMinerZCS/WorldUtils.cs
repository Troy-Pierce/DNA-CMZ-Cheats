using DNA;
using DNA.CastleMinerZ;
using DNA.CastleMinerZ.AI;
using DNA.CastleMinerZ.Terrain;
using DNA.Net.GamerServices;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleMinerZCS
{
    class WorldUtils
    {
        public class Blocks
        {
            public static void massEditBlocks(LocalNetworkGamer gamer, IntVector3 point1, IntVector3 point2, BlockTypeEnum type)
            {
                int xMin = point1.X != point2.X ? Math.Min(point1.X, point2.X) : point1.X, yMin = point1.Y != point2.Y ? Math.Min(point1.Y, point2.Y) : point1.Y, zMin = point1.Z != point2.Z ? Math.Min(point1.Z, point2.Z) : point1.Z;
                int xMax = point1.X != point2.X ? Math.Max(point1.X, point2.X) : point1.X, yMax = point1.Y != point2.Y ? Math.Max(point1.Y, point2.Y) : point1.Y, zMax = point1.Z != point2.Z ? Math.Max(point1.Z, point2.Z) : point1.Z;
                for (int x = xMin; x < xMax; x++)
                {
                    for (int y = yMin; y < yMax; y++)
                    {
                        for (int z = zMin; z < zMax; z++)
                        {
                            IntVector3 vectorLocation = new IntVector3(x, y, z);
                            DNA.CastleMinerZ.Net.AlterBlockMessage.Send(gamer, vectorLocation, type);
                            //AlterBlockMessage.Send();
                        }
                    }
                }
            }
        }
        public class Mobs
        {
            public static void spawnHorde(Player player, int amount)
            {
                for (int i = 0; i < amount; i++) {
                    Vector3 vector = player.LocalPosition;
                    vector.Y += 1f;
                    float distance = DNA.CastleMinerZ.AI.EnemyManager.Instance.CalculatePlayerDistance();
                    float midnight = DNA.CastleMinerZ.AI.EnemyManager.Instance.CalculateMidnight(distance, player.LocalPosition.Y);
                    Vector3 value = player.PlayerPhysics.WorldVelocity;
                    value *= 5f;
                    EnemyTypeEnum zombie = EnemyType.GetZombie(distance);
                    int spawnRadius = EnemyType.Types[(int)zombie].SpawnRadius;
                    vector.X += value.X + (float)DNA.CastleMinerZ.AI.EnemyManager.Instance._rnd.Next(-spawnRadius, spawnRadius + 1);
                    vector.Z += value.Z + (float)DNA.CastleMinerZ.AI.EnemyManager.Instance._rnd.Next(-spawnRadius, spawnRadius + 1);
                    if (BlockTerrain.Instance.RegionIsLoaded(vector))
                    {
                        vector = BlockTerrain.Instance.FindTopmostGroundLocation(vector);
                        IntVector3 a = new IntVector3((int)vector.X, (int)vector.Y - 1, (int)vector.Z);
                        int num = BlockTerrain.Instance.MakeIndexFromWorldIndexVector(a);
                        BlockType type = Block.GetType(BlockTerrain.Instance._blocks[num]);
                        DNA.CastleMinerZ.AI.EnemyManager.Instance._distanceEnemiesLeftToSpawn--;
                        if (type._type == BlockTypeEnum.SpaceRock || type._type == BlockTypeEnum.SpaceRockInventory)
                        {
                            return;
                        }
                        DNA.CastleMinerZ.Net.SpawnEnemyMessage.Send((LocalNetworkGamer)player.Gamer, vector, zombie, midnight, DNA.CastleMinerZ.AI.EnemyManager.Instance.MakeNextEnemyID(), DNA.CastleMinerZ.AI.EnemyManager.Instance._rnd.Next(), Vector3.Zero, 0, null);
                    };
                }
            }
        }
    }
}
