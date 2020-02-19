using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez.Textures;

namespace Nez.Atlases
{
    public class AtlasAnimator : RenderableComponent, IUpdatable
    {
        public enum State
        {
            Paused,
            Playing,
            Completed
        }

        public Atlas Atlas
        {
            get;
            set;
        }

        public int Animation
        {
            get => _animation.AnimationIndex;
            set => _animation.AnimationIndex = value;
        }

        public State AnimationState
        {
            get;
            private set;
        } = State.Paused;

        public LoopMode LoopMode
        {
            get => _loopMode;
            set => _loopMode = value;
        }

        /// <summary>
        /// Animation playback speed
        /// </summary>
        public float Speed
        {
            get;
            set;
        } = 1;

        public int CurrentFrame
        {
            get => _animation.CurrentFrame;
            private set => _animation.CurrentFrame = value;
        }

        public override Color Color
        {
            get => _animation.Color;
            set => _animation.Color = value;

        }

        public SpriteEffects SpriteEffects
        {
            get => _animation.SpriteEffects;
            set => _animation.SpriteEffects = value;
        }


        public override float Width
        {
            get => Atlas.Rectangles[_animation.Index].Width;
        }

        public override float Height
        {
            get => Atlas.Rectangles[_animation.Index].Height;
        }

        public override RectangleF Bounds
        {
            get
            {
                if (_areBoundsDirty)
                {
                    var scale = new Vector2
                    (
                        SpriteEffects == SpriteEffects.FlipHorizontally ? Transform.Scale.X * -1 : Transform.Scale.X,
                        SpriteEffects == SpriteEffects.FlipVertically ? Transform.Scale.Y * -1 : Transform.Scale.Y
                    );

                    _bounds.CalculateBounds(Transform.Position, LocalOffset, Atlas.Origins[_animation.Index],
                                            scale, Transform.Rotation, Width, Height);
                    _areBoundsDirty = false;
                }
                return _bounds;
            }
        }

        private AtlasAnimation _animation;
        private float _elapsedTime;
        private LoopMode _loopMode = LoopMode.Loop;

        public AtlasAnimator(Atlas atlas, int animation)
        {
            Atlas = atlas;
            _animation = new AtlasAnimation(animation);
        }

        public override void Initialize()
        {
            _animation.IndexChanged += OnIndexChanged;
            _animation.SpriteEffectsChanged += OnSpriteEffectsChanged;
            _animation.FrameChanged += OnFrameChanged;
            _animation.AnimationChanged += OnAnimationChanged;
        }

        public void OnIndexChanged(object sender, EventArgs args)
        {
            _areBoundsDirty = true;
        }

        public void OnFrameChanged(object sender, EventArgs args)
        {
            _animation.Index = Atlas.Animations[Animation][CurrentFrame];
        }

        public void OnAnimationChanged(object sender, EventArgs args)
        {
            _animation.CurrentFrame = 0;
        }

        public void OnSpriteEffectsChanged (object sender, EventArgs args)
        {
            _areBoundsDirty = true;
        }

        public bool IsPlaying => AnimationState == State.Playing;
        public bool IsPaused => AnimationState == State.Paused;
        public bool IsCompleted => AnimationState == State.Completed;

        public void Play () => AnimationState = State.Playing;

        public void Pause () => AnimationState = State.Paused;

        public void Update()
        {
            if (AnimationState != State.Playing)
                return;

            var secondsPerFrame = 1 / (Atlas.Framerates[Animation] * Speed);
            var iterationDuration = secondsPerFrame * Atlas.Animations[Animation].Length;

            _elapsedTime += Time.DeltaTime;

            // Once and PingPongOnce reset back to Time = 0 once they complete
            if (_loopMode == LoopMode.Once && _elapsedTime > iterationDuration ||
                _loopMode == LoopMode.PingPongOnce && _elapsedTime > iterationDuration * 2)
            {
                AnimationState = State.Completed;
                _elapsedTime = 0;
                CurrentFrame = 0;
                return;
            }

            if (_loopMode == LoopMode.ClampForever && _elapsedTime > iterationDuration)
            {
                AnimationState = State.Completed;
                CurrentFrame = Atlas.Animations[Animation].Length - 1;
                return;
            }

            // figure out which frame we are on
            int i = Mathf.FloorToInt(_elapsedTime / secondsPerFrame);
            int n = Atlas.Animations[Animation].Length;
            if (n > 2 && (_loopMode == LoopMode.PingPong || _loopMode == LoopMode.PingPongOnce))
            {
                // create a pingpong frame
                int maxIndex = n - 1;
                CurrentFrame = maxIndex - System.Math.Abs(maxIndex - i % (maxIndex * 2));
            }
            else
                // create a looping frame
                CurrentFrame = i % n;
        }

        public override void Render(Batcher batcher, Camera camera)
        {
            batcher.Draw(Atlas, _animation, Transform.Position + LocalOffset, Transform.Rotation,
                        Transform.Scale, LayerDepth);
        }


    }

}