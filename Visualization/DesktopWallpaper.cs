using OsuWallpaperPlayer.Songs;
using osu.Framework.Graphics.Textures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using OpenTK.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Effects;
using osu.Framework.Input;
using OpenTK;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics.OpenGL.Textures;
using System.Drawing;

namespace OsuWallpaperPlayer.Visualization
{
    public class DesktopWallpaper
    {
        public CustomPaper Application { get; }

        public DrawSizePreservingFillContainer PlayerDrawable { get; }

        public PlayInfoContainer PlayInfoContainer { get; }
        public ParallaxContainer BackgroundContainer { get; }

        private Box BackgroundBox { get; set; }

        private Texture background;
        public Texture Background
        {
            get => background;

            private set
            {
                background = value;

                if (BackgroundBox != null)
                    BackgroundBox.FadeOutFromOne(1000).Expire();

                BackgroundContainer.Add(BackgroundBox = CreateBackgroundBox());
                BackgroundBox.FadeInFromZero(1000);
            }
        }

        public DesktopWallpaper(CustomPaper application)
        {
            Application = application;

            PlayInfoContainer = new PlayInfoContainer(application.SongPlayer)
            {
                ParallaxAmount = 0.02f,
                Depth = -3,

                Scale = new Vector2(0.8f, 0.8f)
            };

            PlayInfoContainer.ScaleTo(1, 1000f, Easing.OutQuart);

            BackgroundContainer = new ParallaxContainer()
            {
                RelativeSizeAxes = Axes.Both,
                ParallaxAmount = 0.0125f,
                Depth = -1,

                Children = new[]
                {
                    new Box()
                    {
                        RelativeSizeAxes = Axes.Both,
                        Depth = -2,
                        Colour = Color4.Black,
                        Alpha = 0.33f
                    }
                }
            };

            PlayerDrawable = new DrawSizePreservingFillContainer()
            {
                RelativeSizeAxes = Axes.Both,
                Children = new Drawable[]
                {
                    PlayInfoContainer,
                    BackgroundContainer,
                    new Box()
                    {
                        RelativeSizeAxes = Axes.Both
                    }
                }
            };
        }

        private SongInfo currentSongInfo;

        public SongInfo CurrentSongInfo
        {
            get => currentSongInfo;

            set
            {
                currentSongInfo = value;

                try
                {
                    PlayInfoContainer.SongInfo = value;
                    FileStream stream = new FileStream(value.Background, FileMode.Open);
                    Background = TextureLoader.FromStream(stream);

                    Color4 color = Color4.White;
                    Random rnd = new Random();
                    using (Bitmap bitmap = new Bitmap(stream))
                    {
                        Color accentColor = bitmap.GetPixel((int)(rnd.NextDouble() * bitmap.Width), (int)(rnd.NextDouble() * bitmap.Height));
                        color = new Color4(accentColor.R, accentColor.G, accentColor.B, 255).Lighten(1.33f);
                    }

                    PlayInfoContainer.Visualizer.AccentColour = color.Opacity(0.5f);
                    PlayInfoContainer.CircularBox.Colour = color.Lighten(1.25f).Opacity(0.75f);
                    PlayInfoContainer.TriangleContainer.TriangleColour= PlayInfoContainer.ProgressBar.Colour = color.Darken(1.25f).Opacity(0.75f);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error " + e);
                    Background = Texture.WhitePixel;
                }
            }
        }

        private Box CreateBackgroundBox()
        {
            return new Box()
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,

                Depth = -1,

                Texture = Background,
                RelativeSizeAxes = Axes.Both,
                FillAspectRatio = ((float) Background.Width / Background.Height),
                FillMode = FillMode.Fill
            };
        }
    }
}
