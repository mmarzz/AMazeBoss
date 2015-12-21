﻿using Entitas;

namespace Assets.EntitasRefactor.Preview
{
    public abstract class PreviewChangedSystem : ISetPool, IEnsureComponents
    {
        protected Group PreviewGroup;
        protected Pool Pool;

        public IMatcher ensureComponents { get { return Matcher.Input; } }

        public void SetPool(Pool pool)
        {
            Pool = pool;
        }
    }
}