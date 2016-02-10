﻿using Entitas;

namespace Assets.LevelEditor.Preview
{
    public abstract class PreviewChangedSystem : ISetPool, IEnsureComponents
    {
        protected Group PreviewGroup;
        protected Pool Pool;

        public IMatcher ensureComponents { get { return GameMatcher.Input; } }

        public void SetPool(Pool pool)
        {
            Pool = pool;
        }
    }
}