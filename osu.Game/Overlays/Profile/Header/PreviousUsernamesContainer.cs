﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Game.Graphics;
using osu.Game.Users;
using osuTK;
using System;
using System.Linq;

namespace osu.Game.Overlays.Profile.Header
{
    public class PreviousUsernamesContainer : CompositeDrawable
    {
        private const int duration = 200;
        private const int margin = 10;
        private const int width = 310;
        private const int move_offset = 15;

        public readonly Bindable<User> User = new Bindable<User>();

        private readonly ContentContainer contentContainer;

        public PreviousUsernamesContainer()
        {
            HoverIconContainer hoverIcon;

            AutoSizeAxes = Axes.Y;
            Width = width;

            AddRangeInternal(new Drawable[]
            {
                contentContainer = new ContentContainer(),
                hoverIcon = new HoverIconContainer(),
            });

            hoverIcon.ActivateHover += () =>
            {
                contentContainer.Show();
                this.MoveToY(-move_offset, duration, Easing.OutQuint);
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            User.BindValueChanged(onUserChanged, true);
        }

        private void onUserChanged(ValueChangedEvent<User> user)
        {
            contentContainer.Text = string.Empty;

            var usernames = user.NewValue?.PreviousUsernames;

            if (usernames?.Any() ?? false)
            {
                contentContainer.Text = string.Join(", ", usernames);
                Show();
                return;
            }

            Hide();
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            base.OnHoverLost(e);
            contentContainer.Hide();
            this.MoveToY(0, duration, Easing.OutQuint);
        }

        private class ContentContainer : VisibilityContainer
        {
            private const int header_height = 20;
            private const int content_padding = 40;

            public string Text
            {
                set => usernames.Text = value;
            }

            private readonly TextFlowContainer usernames;
            private readonly Box background;

            public ContentContainer()
            {
                AutoSizeAxes = Axes.Y;
                RelativeSizeAxes = Axes.X;
                Masking = true;
                AlwaysPresent = true;
                CornerRadius = 5;
                Children = new Drawable[]
                {
                    background = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                    },
                    new FillFlowContainer
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Direction = FillDirection.Vertical,
                        Spacing = new Vector2(0, 5),
                        Children = new Drawable[]
                        {
                            new Container
                            {
                                RelativeSizeAxes = Axes.X,
                                Height = header_height,
                                Children = new Drawable[]
                                {
                                    new SpriteText
                                    {
                                        Anchor = Anchor.BottomLeft,
                                        Origin = Anchor.BottomLeft,
                                        Padding = new MarginPadding { Left = content_padding },
                                        Text = @"formerly known as",
                                        Font = OsuFont.GetFont(size: 10, italics: true)
                                    }
                                }
                            },
                            usernames = new TextFlowContainer(s => s.Font = OsuFont.GetFont(size: 12, weight: FontWeight.Bold, italics: true))
                            {
                                RelativeSizeAxes = Axes.X,
                                AutoSizeAxes = Axes.Y,
                                Direction = FillDirection.Full,
                                Spacing = new Vector2(0, 5),
                                Padding = new MarginPadding { Left = content_padding, Bottom = margin },
                            },
                        }
                    }
                };
            }

            [BackgroundDependencyLoader]
            private void load(OsuColour colours)
            {
                background.Colour = colours.GreySeafoamDarker;
            }

            protected override void PopIn() => this.FadeIn(duration, Easing.OutQuint);

            protected override void PopOut() => this.FadeOut(duration, Easing.OutQuint);
        }

        private class HoverIconContainer : Container
        {
            public Action ActivateHover;

            public HoverIconContainer()
            {
                AutoSizeAxes = Axes.Both;
                Child = new SpriteIcon
                {
                    Margin = new MarginPadding(margin) { Top = 6 },
                    Size = new Vector2(15),
                    Icon = FontAwesome.Solid.IdCard,
                };
            }

            protected override bool OnHover(HoverEvent e)
            {
                ActivateHover?.Invoke();
                return base.OnHover(e);
            }
        }
    }
}
