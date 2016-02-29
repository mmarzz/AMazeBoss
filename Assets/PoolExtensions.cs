﻿using System;
using System.Collections.Generic;
using System.Linq;
using Entitas;

namespace Assets
{
    public static class PoolExtensions
    {
        public static UnityEngine.Camera GetCamera(this Pool pool)
        {
            var cameras = Pools.game.GetEntities(GameMatcher.Camera);
            return cameras.SingleEntity().camera.Value;
        }

        public static void SwitchCurse(this Pool pool)
        {
            var hero = pool.GetHero();
            var activeBoss = pool.GetActiveBoss();

            if (activeBoss != null)
            {
                hero.isCursed = !hero.isCursed;
                activeBoss.isCursed = !activeBoss.isCursed;
            }
        }

        public static Entity GetHero(this Pool pool)
        {
            return pool.GetEntities(GameMatcher.Hero).SingleEntity();
        }

        public static Entity GetActiveBoss(this Pool pool)
        {
            try
            {
                var currentPuzzleArea = GetCurrentPuzzleArea(pool);
                return pool
                    .GetEntities(GameMatcher.Boss)
                    .Single(x => !x.isDead && x.id.Value == currentPuzzleArea.bossConnection.BossId);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static Entity GetCurrentPuzzleArea(this Pool pool)
        {
            var hero = pool.GetHero();
            return pool.GetEntityAt(hero.position.Value, x => x.isPuzzleArea);
        }

        public static Entity GetTileAt(this Pool pool, TilePos position)
        {
            return pool.GetEntityAt(position, x => x.gameObject.Type == ObjectType.Tile);
        }

        public static Entity GetItemAt(this Pool pool, TilePos position)
        {
            return pool.GetEntityAt(position, x => x.gameObject.Type == ObjectType.Item);
        }

        public static Entity GetAreaAt(this Pool pool, TilePos position)
        {
            return pool.GetEntityAt(position, x => x.gameObject.Type == ObjectType.Area);
        }

        public static void KnockObjectsInFront(this Pool pool, TilePos position, TilePos forwardDirection, bool immediate)
        {
            pool.GetEntitiesAt(position + forwardDirection, x => x.gameObject.Type == ObjectType.Area && x.isBlockingTile)
                .ToList()
                .ForEach(x => x.ReplaceKnocked(forwardDirection, immediate));
        }

        public static bool OpenTileAt(this Pool pool, TilePos position)
        {
            var entitiesAtPosition = pool.GetEntitiesAt(position).ToList();
            return entitiesAtPosition.Count > 0 && entitiesAtPosition.All(x => !x.isBlockingTile);
        }

        public static bool PushableItemAt(this Pool pool, TilePos position)
        {
            var entitiesAtPosition = pool.GetEntitiesAt(position, x => x.isBox).ToList();
            return entitiesAtPosition.Count > 0;
        }

        public static void SafeDeleteAll(this Pool pool, IMatcher matcher = null)
        {
            var entities = matcher != null ? pool.GetEntities(matcher) : pool.GetEntities();
            entities.Where(x => !x.isPreview).ToList().DoForAll(x => x.IsDestroyed(true));
        }

        public static void SafeDeleteLevel(this Pool pool)
        {
            Pools.game.SafeDeleteAll(GameMatcher.GameObject);
        }

        public static Entity FindChildFor(this Pool pool, Entity entity)
        {
            return pool.FindChildrenFor(entity).SingleEntity();
        }

        public static List<Entity> FindChildrenFor(this Pool pool, Entity entity)
        {
            return pool
                .GetEntities(GameMatcher.Child)
                .Where(x => x.child.ParentId == entity.id.Value)
                .ToList();
        }

        public static Entity GetEntityAt(this Pool pool, TilePos position, Func<Entity, bool> entityMatcher = null)
        {
            var entitiesAtPosition = pool
                .GetEntitiesAt(position, entityMatcher)
                .ToList();

            if (entitiesAtPosition.Count() > 1)
            {
                throw new MoreThanOneMatchException(entitiesAtPosition);
            }

            return entitiesAtPosition.SingleOrDefault();
        }

        public static List<Entity> GetEntitiesAt(this Pool pool, TilePos position, Func<Entity, bool> entityMatcher = null)
        {
            if (!pool.objectPositionCache.Cache.ContainsKey(position))
                return new List<Entity>();

            return pool.objectPositionCache
                .Cache[position]
                .Where(x => x.hasGameObject && !x.isDestroyed && !x.isPreview && (entityMatcher == null || entityMatcher(x)))
                .ToList();
        }

        public class MoreThanOneMatchException : Exception
        {
            public MoreThanOneMatchException(params object[] matched) :
                base("Found multiple matches: " + string.Join(",", matched.Select(x => x.ToString()).ToArray()))
            {
            }
        }

        public static List<Entity> DoForAll(this List<Entity> entities, Action<Entity> action)
        {
            entities.ForEach(action);
            return entities;
        }

        public static void DoForAllAtPosition(this Pool pool, TilePos position, Action<Entity> entityAction)
        {
            pool.objectPositionCache.Cache[position].DoForAll(entityAction);
        }
    }
}