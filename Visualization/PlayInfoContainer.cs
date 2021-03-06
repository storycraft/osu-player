﻿using OpenTK.Graphics;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu_player.Songs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace osu_player.Visualization
{
    public class PlayInfoContainer : ParallaxContainer
    {
        public BeatmapSongPlayer Player { get; }

        public AudioVisualizer Visualizer { get; }

        public Box CircularBox { get; }

        public CircularContainer Circular { get; }

        public TriangleContainer TriangleContainer { get; }

        private Container InfoContainer { get; }

        public SpriteText TitleText { get; set; }
        public SpriteText ArtistText { get; set; }

        public SpriteText ProgressText { get; set; }

        public CircularProgress ProgressBar { get; }

        public PlayInfoContainer(BeatmapSongPlayer player)
        {
            Player = player;

            Children = new Drawable[]{
                Visualizer = new AudioVisualizer(player)
                {
                    Origin = Anchor.Centre,
                    Anchor = Anchor.Centre,
                    Size = new OpenTK.Vector2(450f, 450f),
                    Depth = -3
                },
                Circular = new CircularContainer()
                {
                    Origin = Anchor.Centre,
                    Anchor = Anchor.Centre,

                    Size = new OpenTK.Vector2(450f, 450f),

                    Masking = true,

                    CornerRadius = 175f,

                    Depth = -4,

                    Children = new Drawable[]
                    {
                        ProgressBar = new CircularProgress()
                        {
                            Margin = new MarginPadding(50f),
                            Origin = Anchor.Centre,
                            Anchor = Anchor.Centre,

                            RelativeSizeAxes = Axes.Both,

                            Colour = Color4.White,
                            Depth = -4
                        },
                        InfoContainer = new CircularContainer()
                        {
                            Size = new OpenTK.Vector2(400f, 400f),
                            Margin = new MarginPadding(50f),

                            Masking = true,
                            CornerRadius = 400f / 2f,

                            Origin = Anchor.Centre,
                            Anchor = Anchor.Centre,

                            EdgeEffect = new EdgeEffectParameters
                            {
                                Type = EdgeEffectType.Shadow,
                                Colour = Color4.Black.Lighten(1.15f).Opacity(75),
                                Radius = 3f
                            },

                            Depth = -4,

                            Children = new Drawable[]
                            {
                                TriangleContainer = new TriangleContainer()
                                {
                                    Origin = Anchor.Centre,
                                    Anchor = Anchor.Centre,

                                    Depth = -5,

                                    RelativeSizeAxes = Axes.Both,
                                    Padding = new MarginPadding(25f),

                                    Children = new Drawable[]
                                    {
                                        TitleText = CreateTitleText("Title"),
                                        ArtistText = CreateArtistText("Artist"),
                                        ProgressText = new SpriteText()
                                        {
                                            Origin = Anchor.Centre,
                                            Anchor = Anchor.BottomCentre,

                                            Font = "OpenSans",
                                            Colour = Color4.Black,
                                            TextSize = 30f,
                                            Text = "0:00 / 0:00",
                                            Depth = -6
                                        },
                                        new Box()
                                        {
                                            RelativeSizeAxes = Axes.Both
                                        }
                                    }
                                },
                                new Box()
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Colour = Color4.White
                                }
                            }
                        },
                        CircularBox = new Box()
                        {
                            RelativeSizeAxes = Axes.Both
                        }
                    },
                }
            };

            EdgeEffect = new EdgeEffectParameters
            {
                Colour = Color4.Black.Lighten(1.25f).Opacity(0.6f),
                Radius = 5f
            };
        }

        private SongInfo songInfo;

        public SongInfo SongInfo
        {
            get
            {
                return songInfo;
            }

            set
            {
                songInfo = value;

                TitleText.FadeOutFromOne(1000).Expire();
                ArtistText.FadeOutFromOne(1000).Expire();

                InfoContainer.Add(TitleText = CreateTitleText(value.Title));
                InfoContainer.Add(ArtistText = CreateArtistText(value.ArtistName));

                TitleText.FadeInFromZero(1000);
                ArtistText.FadeInFromZero(1000);
            }
        }

        protected override void Update()
        {
            ProgressBar.Current.Value = (float)Player.CurrentTime / Math.Max(Player.PlayTime, 1);

            string progressText = "";

            int seconds = (int)(Player.CurrentTime / 1000);
            int minutes = seconds / 60;
            int hours = minutes / 60;

            if (hours > 0)
                progressText += hours + ":";

            progressText += minutes % 60 + ":";
            progressText += seconds % 60;

            progressText += " / ";

            seconds = (int)(Player.PlayTime / 1000);
            minutes = seconds / 60;
            hours = minutes / 60;

            if (hours > 0)
                progressText += hours + ":";

            progressText += minutes % 60 + ":";
            progressText += seconds % 60;

            ProgressText.Text = progressText;

            base.Update();
        }

        protected SpriteText CreateTitleText(string title)
        {
            return new SpriteText()
            {
                Origin = Anchor.Centre,
                Anchor = Anchor.Centre,

                Font = "OpenSans",
                Colour = Color4.Black,
                TextSize = 45f,
                Text = title,
                Depth = -6
            };
        }

        protected SpriteText CreateArtistText(string artist)
        {
            return new SpriteText()
            {
                Origin = Anchor.Centre,
                Anchor = Anchor.Centre,
                Y = 40,

                Font = "OpenSans",
                Colour = Color4.Black,
                TextSize = 25f,
                Text = artist,
                Depth = -6
            };
        }
    }
}