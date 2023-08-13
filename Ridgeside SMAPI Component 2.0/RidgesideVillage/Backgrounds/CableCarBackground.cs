﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewModdingAPI.Events;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace RidgesideVillage
{
    public class CableCarBackground : Background
    {
        static IModHelper Helper;
        static IMonitor Monitor;

        private Texture2D bg;
        private static Vector2 speed;
        private static Vector2 offset;

        internal static void Initialize(IMod ModInstance)
        {
            Helper = ModInstance.Helper;
            Monitor = ModInstance.Monitor;

            Helper.Events.Display.RenderingWorld += OnRenderingWorld;
            Helper.Events.Display.RenderedWorld += OnRenderedWorld;
        }

        public CableCarBackground()
        : base(new Color(0, 0, 0), false)
        {
            Log.Trace($"RSV: Creating Cable Car bg");
            Game1.player.eventsSeen.Add(75160060);
            Game1.player.eventsSeen.Add(75160404);
            Game1.player.eventsSeen.Add(75160175);
            Game1.player.eventsSeen.Add(75160176);
            bg = Helper.ModContent.Load<Texture2D>(PathUtilities.NormalizePath("assets/CableCarBg.png"));
            speed = new Vector2(0.5f, 0.31f);
            offset = new Vector2(bg.Width, bg.Height) * -2;
        }

        public void Update(xTile.Dimensions.Rectangle viewport)
        {
            Event current = Game1.CurrentEvent;
            if (current == null)
                return;
            if (current.id != 94621001)
                return;
            Log.Trace($"RSV: Current command: " + current.currentCommand);
            if (current.CurrentCommand > 75)
                return;
            Vector2 new_offset = offset + speed;
            if (new_offset.X > 0 && new_offset.Y > 0)
                return;
            offset = new_offset;
            Log.Trace($"RSV: Cable car offset: " + offset.X + " " + offset.Y);
        }

        public void Draw(SpriteBatch b)
        {
            try
            {
                Game1.spriteBatch.End();
                Game1.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);

                Rectangle display = new Rectangle(0, 0, Game1.viewport.Width, Game1.viewport.Height);
                b.Draw(bg, Game1.GlobalToLocal(Game1.viewport, offset), display, Color.White, 0, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 1);

                Game1.spriteBatch.End();
                Game1.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
            }
            catch (Exception ex)
            {
                Log.Error($"RSV: Error drawing Cable Car bg:\n\n{ex}");
                Game1.spriteBatch.End();
                Game1.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
            }
        }

        private static void OnRenderingWorld(object sender, RenderingWorldEventArgs e)
        {
            if (Game1.background is CableCarBackground)
            {
                // This part doesn't do anything normally (https://github.com/MonoGame/MonoGame/issues/5441),
                // but SpriteMaster makes it work. So need this for compatibility.
                if (Game1.graphics.PreferredDepthStencilFormat != DepthFormat.Depth24Stencil8)
                {
                    Game1.graphics.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;
                    Game1.graphics.ApplyChanges();
                }

                BgUtils.DefaultStencilOverride = BgUtils.StencilDarken;
                Game1.graphics.GraphicsDevice.Clear(ClearOptions.Stencil, Color.Transparent, 0, 0);
            }
        }

        private static void OnRenderedWorld(object sender, RenderedWorldEventArgs e)
        {
            BgUtils.DefaultStencilOverride = null;
        }
    }
}