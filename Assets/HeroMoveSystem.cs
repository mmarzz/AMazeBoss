﻿using System.Collections.Generic;
using System.Linq;
using Entitas;
using UnityEngine;

namespace Assets
{
    public class HeroMoveSystem : IExecuteSystem, ISetPool
    {
        private Pool _pool;
        private Group _positionGroup;
        private Group _cameraGroup;

        private readonly Dictionary<KeyCode, TilePos> _moveDirections = new Dictionary<KeyCode, TilePos>
            {
                { KeyCode.UpArrow, new TilePos(0, 1) },
                { KeyCode.DownArrow, new TilePos(0, -1) },
                { KeyCode.LeftArrow, new TilePos(-1, 0) },
                { KeyCode.RightArrow, new TilePos(1, 0) }
            };

        private Group _heroGroup;

        public void SetPool(Pool pool)
        {
            _pool = pool;
            _heroGroup = pool.GetGroup(Matcher.Hero);
            _positionGroup = pool.GetGroup(Matcher.Position);
            _cameraGroup = pool.GetGroup(Matcher.AllOf(Matcher.Camera, Matcher.Rotation));
        }

        public void Execute()
        {
            var hero = _heroGroup.GetSingleEntity();
            var inputMoveDirection = GetInputMoveDirection();

            var hasMoved = inputMoveDirection.Length() > 0;
            var newPosition = inputMoveDirection + hero.position.Value;
            var canMove = _pool.CanMoveTo(newPosition) && !hero.isCursed;
            if (hasMoved && canMove)
            {
                if (hero.IsMoving())
                {
                    hero.ReplaceQueuedPosition(newPosition);
                }
                else
                {
                    hero.ReplacePosition(newPosition);
                }
            }
            else if(hasMoved)
            {
                _pool.KnockObjectsInFront(hero.position.Value, inputMoveDirection);
            }
        }

        private TilePos GetInputMoveDirection()
        {
            var inputMoveDirection = new TilePos(0, 0);

            foreach (var moveDirection in _moveDirections)
            {
                if (Input.GetKeyDown(moveDirection.Key))
                {
                    inputMoveDirection = moveDirection.Value;
                }
            }

            if (_cameraGroup.count != 0)
            {
                var camera = _cameraGroup.GetSingleEntity();
                inputMoveDirection = inputMoveDirection.Rotate(camera.rotation.Value);
            }

            return inputMoveDirection;
        }
    }
}