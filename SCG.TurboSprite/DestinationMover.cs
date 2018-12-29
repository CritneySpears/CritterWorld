﻿#region copyright
/*
* Copyright (c) 2008, Dion Kurczek
* Modifications copyright (c) 2018, Dave Voorhis
* All rights reserved.
*
* Redistribution and use in source and binary forms, with or without
* modification, are permitted provided that the following conditions are met:
*     * Redistributions of source code must retain the above copyright
*       notice, this list of conditions and the following disclaimer.
*     * Redistributions in binary form must reproduce the above copyright
*       notice, this list of conditions and the following disclaimer in the
*       documentation and/or other materials provided with the distribution.
*     * Neither the name of the <organization> nor the
*       names of its contributors may be used to endorse or promote products
*       derived from this software without specific prior written permission.
*
* THIS SOFTWARE IS PROVIDED BY DION KURCZEK ``AS IS'' AND ANY
* EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
* DISCLAIMED. IN NO EVENT SHALL DION KURCZEK BE LIABLE FOR ANY
* DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
* (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
* LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
* ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
* (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
* SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
#endregion
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCG.TurboSprite
{
    // Move a Sprite toward a destination.
    public class DestinationMover : IMover
    {
        private Sprite _sprite;
        private float _destX;
        private float _destY;

        // Sprite's speed
        public float Speed { get; set; }

        public float SpeedX { get; set; }

        public float SpeedY { get; set; }

        // Sprite's destination
        public float DestX
        {
            get
            {
                return _destX;
            }
            set
            {
                _destX = value;
                CalculateVectors();
            }
        }

        public float DestY
        {
            get
            {
                return _destY;
            }
            set
            {
                _destY = value;
                CalculateVectors();
            }
        }

        public PointF DestinationF
        {
            get
            {
                return new PointF(_destX, _destY);
            }
            set
            {
                _destX = value.X;
                _destY = value.Y;
                CalculateVectors();
            }
        }

        public Point Destination
        {
            get
            {
                return new Point((int)_destX, (int)_destY);
            }
            set
            {
                _destX = value.X;
                _destY = value.Y;
                CalculateVectors();
            }
        }

        // Should the sprite stop moving once it reaches its destination?
        public bool StopAtDestination { get; set; } = true;

        // Calculate X/Y movement vectors based on speed and destination
        private void CalculateVectors()
        {
            if (_sprite == null)
            {
                return;
            }
            float distance = Math.Abs(DestX - _sprite.X) + Math.Abs(DestY - _sprite.Y);
            if (distance > 0)
            {
                float PctX = Math.Abs(DestX - _sprite.X) / distance;
                float PctY = Math.Abs(DestY - _sprite.Y) / distance;
                SpeedX = Speed * PctX;
                SpeedY = Speed * PctY;
                if (DestX < _sprite.X)
                {
                    SpeedX = -SpeedX;
                }
                if (DestY < _sprite.Y)
                {
                    SpeedY = -SpeedY;
                }
            }
            else
            {
                SpeedX = Speed / 2;
                SpeedY = SpeedX;
            }
        }

        // events
        public event EventHandler<SpriteEventArgs> SpriteReachedDestination;
        public event EventHandler<SpriteEventArgs> SpriteMoved;

        // Move the sprite, called by SpriteEngine's MoveSprite method
        public void MoveSprite(Sprite sprite)
        {
            if (_sprite == null)
            {
                _sprite = sprite;
                CalculateVectors();
            }
            if (SpeedX == 0 && SpeedY == 0)
            {
                return;
            }
            int oldX = (int)_sprite.X;
            int oldY = (int)_sprite.Y;
            if (!StopAtDestination)
            {
                // Do not check destination, just move the sprite
                _sprite.X += SpeedX;
                _sprite.Y += SpeedY;
            }
            else
            {
                // Handle reaching destination
                float TempX = _sprite.X + SpeedX;
                _sprite.X = ((SpeedX > 0 && TempX > DestX) || (SpeedX < 0 && TempX < DestX)) ? DestX : _sprite.X + SpeedX;
                float TempY = _sprite.Y + SpeedY;
                _sprite.Y = ((SpeedY > 0 && TempY > DestY) || (SpeedY < 0 && TempY < DestY)) ? DestY : _sprite.Y + SpeedY;
            }
            // If sprite moved, alert listeners
            if (SpriteMoved != null && ((int)_sprite.X != oldX || (int)_sprite.Y != oldY))
            {
                SpriteMoved(this, new SpriteEventArgs(_sprite));
            }
            // If sprite has reached its target destination, alert listeners
            if (SpriteReachedDestination != null && sprite.Position == Destination)
            {
                SpriteReachedDestination(this, new SpriteEventArgs(_sprite));
            }
        }
    }
}
