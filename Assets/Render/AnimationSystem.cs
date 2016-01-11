﻿using Entitas;
using DG.Tweening;
using UnityEngine;

namespace Assets.Render
{
    public class AnimationSystem : IInitializeSystem, ISetPool
    {
        private Group _positionable;
        private Group _trapActivatedGroup;
        private Group _trapLoadedGroup;
        private const int TrapActivateTime = 1;
        private const float BossMoveTime = 0.5f;

        public void SetPool(Pool pool)
        {
            _positionable = pool.GetGroup(Matcher.Position);
            _trapLoadedGroup = Pools.pool.GetGroup(Matcher.AllOf(Matcher.Animator, Matcher.SpikeTrap));
            _trapActivatedGroup = pool.GetGroup(Matcher.AllOf(Matcher.Animator, Matcher.TrapActivated));
        }

        public void Initialize()
        {
            _positionable.OnEntityUpdated += (g, e, i, pc, nc) => PositionChanged(e);
            _trapLoadedGroup.OnEntityAdded += (g, e, i, nc) => TrapLoaded(e);
            _trapLoadedGroup.OnEntityRemoved += (g, e, i, pc) => TrapLoaded(e);
            _trapActivatedGroup.OnEntityAdded += (g, e, i, nc) => TrapActivated(e);
        }

        private void TrapLoaded(Entity entity)
        {
            entity.animator.Value.SetBool("Loaded", entity.spikeTrap.IsLoaded);
        }

        private void TrapActivated(Entity entity)
        {
            entity.animator.Value.SetTrigger("Activated");
            entity.ReplaceActingTime(TrapActivateTime, () => entity.IsTrapActivated(false));
        }

        private void PositionChanged(Entity entity)
        {
            var transform = entity.view.Value.transform;
            var newPosition = entity.position.Value.ToV3();
            transform.DOMove(newPosition, BossMoveTime).SetEase(Ease.Linear);

            if(entity.hasAnimator)
            {
                entity.animator.Value.SetBool("IsMoving", true);
                entity.ReplaceActingTime(BossMoveTime, () => entity.animator.Value.SetBool("IsMoving", false));
            }
            else
            {
                entity.ReplaceActingTime(BossMoveTime, () => { });
            }
            if (entity.IsMoving())
            {
                transform.rotation = Quaternion.LookRotation(newPosition - transform.position, Vector3.up);
            }
        }
    }
}