﻿using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace Assets
{
    public class BottomSpawnerSystem : IReactiveSystem, ISetPool
    {
        private Pool _pool;

        public TriggerOnEvent trigger { get { return GameMatcher.Tile.OnEntityAdded(); } }

        public void SetPool(Pool pool)
        {
            _pool = pool;
        }

        public void Execute(List<Entity> entities)
        {
            foreach (var entity in entities)
            {
                AddOrUpdateBottomFor(entity);
            }
        }

        private void AddOrUpdateBottomFor(Entity entity)
        {
            _pool.CreateEntity()
                .SetParent(entity)
                .ReplacePosition(entity.position.Value)
                .ReplaceRotation(Random.Range(0, 4))
                .ReplaceResource("Bottoms/Empty");
        }
    }
}
