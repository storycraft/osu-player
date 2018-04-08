using OpenTK;
using OpenTK.Graphics;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuWallpaperPlayer.Visualization
{
    public class TriangleContainer : Container
    {
        private const float create_chance = 0.1f;

        private const float triangle_min_size = 35f;
        private const float triangle_max_size = 75f;

        private const float triangle_min_speed = 2250f;
        private const float triangle_max_speed = 4500f;

        public ColourInfo TriangleColour { get; set; }

        public TriangleContainer() : base()
        {
            TriangleColour = Color4.Black;
        }

        protected override void Update()
        {
            Random rnd = new Random();

            if (rnd.NextDouble() <= create_chance)
            {
                float size = triangle_min_size + (float) (rnd.NextDouble() * (triangle_max_size - triangle_min_size));

                Triangle triangle = new Triangle()
                {
                    Origin = Anchor.Centre,

                    Colour = TriangleColour,
                    Alpha = 0.015f + (float) (rnd.NextDouble() * 0.135f),
                    Size = new Vector2(size),
                    Position = new Vector2((float) (rnd.NextDouble() * DrawWidth), DrawHeight + size)
                };

                Add(triangle);
                triangle.MoveToY(-size, triangle_min_speed + (triangle_max_speed - triangle_min_speed) * rnd.NextDouble()).Expire();
            }

            base.Update();
        }
    }
}
