﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entitas;

namespace Assets
{
    public class QueuePositionSystem : IExecuteSystem, ISetPool
    {
        private Group _queuedGroup;

        public void SetPool(Pool pool)
        {
            _queuedGroup = pool.GetGroup(Matcher.AllOf(Matcher.QueuedPosition));
        }

        public void Execute()
        {
            foreach (var e in _queuedGroup.GetEntities())
            {
                if (!e.IsMoving())
                {
                    e.ReplacePosition(e.queuedPosition.Value);
                    e.RemoveQueuedPosition();
                }
            }
        }
    }
}