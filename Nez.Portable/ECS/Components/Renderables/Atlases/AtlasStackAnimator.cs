using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez.Textures;

namespace Nez.Atlases
{
    public class AtlasStackAnimator : RenderableComponent, IUpdatable
    {
        /// <summary>
        /// Indexer for easy layer access.
        /// </summary>
        public AtlasAnimation this[int i]
        {
            get => Stack[i];
        }

        public enum State
        {
            Paused,
            Playing,
            Completed
        }

        /// <summary>
        /// Atlas each layer of the stack will render from.
        /// </summary>
        public Atlas Atlas
        {
            get;
            set;
        }

        /// <summary>
        /// Collection of renderable indexes to draw.
        /// </summary>
        public AtlasAnimation[] Stack
        {
            get;
            private set;
        }

        public State AnimationState
        {
            get;
            private set;
        } = State.Paused;

        /// <summary>
        /// Multiplier for the speed of animation playback.
        /// </summary>
        public float Speed
        {
            get;
            set;
        } = 1;

        /// <summary>
        /// Renderable component color override, acts as a get/setter for the entire stack.
        /// </summary>
        public override Color Color
        {
            get => Stack[0].Color;
            set
            {
                foreach (var animation in Stack)
                    animation.Color = value;
            }
        }

        /// <summary>
        /// Handles flipping of the component, acts a get/setter for the entire stack.
        /// </summary>
        public SpriteEffects SpriteEffects
        {
            get => Stack[0].SpriteEffects;
            set
            {
                foreach(var animation in Stack)
                    animation.SpriteEffects = value;
            }
        }

        public override float Width
        {
            get => _stackBounds.Width;
        }

        public override float Height
        {
            get => _stackBounds.Height;
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
                    
                    _bounds.CalculateBounds(Transform.Position, LocalOffset,
                   scale, Transform.Rotation, _stackBounds);

                    _areBoundsDirty = false;
                }

                return _bounds;
            }
        }

        private RectangleF _stackBounds;
        private float _elapsedTime;
        private LoopMode _loopMode = LoopMode.Loop;

        public AtlasStackAnimator(Atlas atlas, params int[] animations)
        {
            Atlas = atlas;
            Stack = new AtlasAnimation[animations.Length];

            for (int i = 0; i < Stack.Length; i++)
                Stack[i] = new AtlasAnimation(animations[i]);
        }

        public override void Initialize()
        {
            // Calculate the initial AABB of our stack of renderables
            CalculateStackBounds();

            // Subscribe to event to be notified of any changes to our stack
            // to recalculate the stack bounds only when needed
            foreach (var animation in Stack)
            {
                animation.Index = Atlas.Animations[animation.AnimationIndex][animation.CurrentFrame];
                animation.IndexChanged += OnIndexChanged;
                animation.FrameChanged += OnFrameChanged;
                animation.AnimationChanged += OnAnimationChanged;
            }
        }

        /// <summary>
        /// Calculates the AABB for the entire stack of renderable indexes.
        /// </summary>
        public void CalculateStackBounds()
        {
            var x = float.MaxValue;
            var y = float.MaxValue;
            var width = float.MinValue;
            var height = float.MinValue;

            for (var i = 0; i < Stack.Length; i++)
            {
                x = Math.Min(-Atlas.Origins[Stack[i].Index].X, x);
                y = Math.Min(-Atlas.Origins[Stack[i].Index].Y, y);
                width = Math.Max(Atlas.Rectangles[Stack[i].Index].Width + x, width + x) - x;
                height = Math.Max(Atlas.Rectangles[Stack[i].Index].Height + y, height + y) - y;
            }

            if (_stackBounds.X != x || _stackBounds.Y != y || _stackBounds.Width != width || _stackBounds.Height != height)
            {
                _stackBounds.X = -x;
                _stackBounds.Y = -y;
                _stackBounds.Width = width;
                _stackBounds.Height = height;

                _areBoundsDirty = true;
            }
        }

        public void OnIndexChanged(object sender, EventArgs args)
        {
            CalculateStackBounds();
        }

        public void OnSpriteEffectsChanged(object sender, EventArgs args)
        {
            CalculateStackBounds();
            _areBoundsDirty = true;
        }

        public void OnFrameChanged(object sender, EventArgs args)
        {
            var s = sender as AtlasAnimation;
            s.Index = Atlas.Animations[s.AnimationIndex][s.CurrentFrame];
        }

        public void OnAnimationChanged(object sender, EventArgs args)
        {
            var s = sender as AtlasAnimation;
            s.CurrentFrame = 0;
        }

        public bool IsPlaying => AnimationState == State.Playing;
        public bool IsPaused => AnimationState == State.Paused;
        public bool IsCompleted => AnimationState == State.Completed;


        public void Play() => AnimationState = State.Playing;

        public void Pause() => AnimationState = State.Paused;


        public void Update()
        {
            if (AnimationState != State.Playing)
                return;

            _elapsedTime += Time.DeltaTime;

            foreach (var animation in Stack)
            {

                var secondsPerFrame = 1 / (Atlas.Framerates[animation.AnimationIndex] * Speed);
                var iterationDuration = secondsPerFrame * Atlas.Animations[animation.AnimationIndex].Length;

                // Once and PingPongOnce reset back to Time = 0 once they complete
                if (_loopMode == LoopMode.Once && _elapsedTime > iterationDuration ||
                    _loopMode == LoopMode.PingPongOnce && _elapsedTime > iterationDuration * 2)
                {
                    AnimationState = State.Completed;
                    _elapsedTime = 0;
                    animation.CurrentFrame = 0;
                    return;
                }

                if (_loopMode == LoopMode.ClampForever && _elapsedTime > iterationDuration)
                {
                    AnimationState = State.Completed;
                    animation.CurrentFrame = Atlas.Animations[animation.AnimationIndex].Length - 1;
                    return;
                }

                // figure out which frame we are on
                int i = Mathf.FloorToInt(_elapsedTime / secondsPerFrame);
                int n = Atlas.Animations[animation.AnimationIndex].Length;
                if (n > 2 && (_loopMode == LoopMode.PingPong || _loopMode == LoopMode.PingPongOnce))
                {
                    // create a pingpong frame
                    int maxIndex = n - 1;
                    animation.CurrentFrame = maxIndex - System.Math.Abs(maxIndex - i % (maxIndex * 2));
                }
                else
                    // create a looping frame
                    animation.CurrentFrame = i % n;
            }
        }

        public override void Render(Batcher batcher, Camera camera)
        {
            foreach (var animation in Stack)
                batcher.Draw(Atlas, animation, Transform.Position + LocalOffset, Transform.Rotation,
                            Transform.Scale, LayerDepth);
        }


    }

}