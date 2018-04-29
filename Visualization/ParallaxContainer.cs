// Copyright (c) 2007-2018 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics;
using osu.Framework.Input;
using OpenTK;
using osu.Framework.MathUtils;

namespace osu_player.Visualization
{
    public class ParallaxContainer : Container, IRequireHighFrequencyMousePosition
    {
        public const float DEFAULT_PARALLAX_AMOUNT = 0.02f;

        public float ParallaxAmount = DEFAULT_PARALLAX_AMOUNT;

        private bool parallaxEnabled;
        public bool ParallaxEnabled {
            get
            {
                return parallaxEnabled;
            }

            set
            {
                parallaxEnabled = value;

                if (IsLoaded)
                {
                    content.Scale = new Vector2(1 + ParallaxAmount);
                }
            }
        }

        public ParallaxContainer()
        {
            ParallaxEnabled = true;
            RelativeSizeAxes = Axes.Both;
            AddInternal(content = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
            });
        }

        private readonly Container content;
        private InputManager input;

        protected override Container<Drawable> Content => content;

        protected override void LoadComplete()
        {
            base.LoadComplete();
            input = GetContainingInputManager();
        }

        protected override void Update()
        {
            base.Update();

            if (ParallaxEnabled)
            {
                Vector2 offset = (input.CurrentState.Mouse == null ? Vector2.Zero : ToLocalSpace(input.CurrentState.Mouse.NativeState.Position) - DrawSize / 2) * ParallaxAmount;

                double elapsed = MathHelper.Clamp(Clock.ElapsedFrameTime, 0, 1000);

                content.Position = Interpolation.ValueAt(elapsed, content.Position, offset, 0, 1000, Easing.OutQuint);
                content.Scale = Interpolation.ValueAt(elapsed, content.Scale, new Vector2(1 + ParallaxAmount), 0, 1000, Easing.OutQuint);
            }
        }
    }
}