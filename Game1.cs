﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using System;

namespace FallingSand
{
    public static class Storage
    {
        public static GraphicsDeviceManager GDM;
        public static ContentManager CM;
        public static SpriteBatch SB;
        public static Game1 GAME;
        public static Texture2D texture;
        public static Rectangle drawRec = new Rectangle(0, 0, 1, 1);
        public static Particle[,] particles;
    }

    public class Particle
    {
        public int x_velocity, y_velocity;
        public Color color;
    }

    public class Game1 : Game
    {
        public int size_x = 512;
        public int size_y = 512;
        public int timer = 0;
        public Game1()
        {
            Storage.GDM = new GraphicsDeviceManager(this);
            Storage.GDM.GraphicsProfile = GraphicsProfile.HiDef;
            Storage.CM = Content;
            Storage.GAME = this;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            IsFixedTimeStep = true;
            TargetElapsedTime = TimeSpan.FromMilliseconds(16.667); //60 frames/sec
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            Storage.GDM.PreferredBackBufferWidth = 512;
            Storage.GDM.PreferredBackBufferHeight = 512;
            Storage.GDM.ApplyChanges();
            Storage.particles = new Particle[size_x, size_y];
            for (int x = 0; x < size_x; x++)
            {
                for (int y = 0; y < size_y; y++)
                {
                    Storage.particles[x, y] = null;
                }
            }

            base.Initialize();
        }

        protected override void LoadContent()
        {
            Storage.SB = new SpriteBatch(GraphicsDevice);
            if (Storage.texture == null)
            {   //set up a general texture we can draw dots with, if required
                Storage.texture = new Texture2D(Storage.GDM.GraphicsDevice, 1, 1);
                Storage.texture.SetData<Color>(new Color[] { Color.SandyBrown });
            }

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            for (int x = 0; x < size_x; x++)
            {
                for (int y = 0; y < size_y; y++)
                {
                    var particle = Storage.particles[x, y];
                    if (particle != null)
                    {
                        particle.y_velocity += 1;
                        if (MathF.Abs(particle.y_velocity) > 1)
                        {
                            particle.y_velocity = 1 * Math.Sign(particle.y_velocity);
                        }

                        particle.x_velocity += 0;
                        if (MathF.Abs(particle.x_velocity) > 1)
                        {
                            particle.x_velocity = 1 * Math.Sign(particle.x_velocity);
                        }

                        int next_x = x + particle.x_velocity;
                        int next_y = y + particle.y_velocity;
                        if (!find_collision(next_x, next_y))
                        {
                            // Go there.
                        }
                        else if (!find_collision(next_x - 1, next_y))
                        {
                            next_x -= 1;
                        }
                        else if (!find_collision(next_x + 1, next_y))
                        {
                            next_x += 1;
                        }
                        // Simulate water.
                        /*
                        else if (!find_collision(next_x - 1, y))
                        {
                            next_x -= 1;
                            next_y = y;
                            particle.y_velocity = 0;
                        }
                        else if (!find_collision(next_x + 1, y))
                        {
                            next_x += 1;
                            next_y = y;
                            particle.y_velocity = 0;
                        }
                        */
                        else
                        {
                            // Can't move.
                            next_x = x;
                            particle.x_velocity = 0;

                            next_y = y;
                            particle.y_velocity = 0;
                        }
                        Storage.particles[x, y] = null;
                        x = next_x;
                        y = next_y;
                        Storage.particles[x, y] = particle;
                    }
                }
            }

            timer += gameTime.ElapsedGameTime.Milliseconds;
            var mouse_state = Mouse.GetState();
            if (mouse_state.LeftButton == ButtonState.Pressed &&  timer > 40)
            {
                timer = 0;
                var x = mouse_state.Position.X * 64 / 512;
                var y = mouse_state.Position.Y * 64 / 512;
                if (x >= 0 && x < size_x && y >= 0 && y < size_y && Storage.particles[x, y] == null)
                {
                    Particle particle = new Particle();
                    Storage.particles[x, y] = particle;
                }
            }

            base.Update(gameTime);
        }

        bool find_collision(int x, int y)
        {
            if (y >= 64)
            {
                return true;
            }
            if (x <= -1 || x >= 64)
            {
                return true;
            }
            return Storage.particles[x, y] != null;
        }

        protected override void Draw(GameTime gameTime)
        {
            SpriteBatch targetBatch = new SpriteBatch(GraphicsDevice);
            RenderTarget2D target = new RenderTarget2D(GraphicsDevice, 64, 64);
            GraphicsDevice.SetRenderTarget(target);

            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            // Storage.SB.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
            Storage.SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);

            int depth = 0;
            for (int x = 0; x < size_x; x++)
            {
                for (int y = 0; y < size_y; y++)
                {
                    depth += 1;
                    Particle particle = Storage.particles[x, y];
                    if (particle != null)
                    {
                        Vector2 pos = new Vector2(x, y);
                        //draw each particle as a sprite
                        Storage.SB.Draw(Storage.texture,
                            pos,
                            Storage.drawRec,
                            Color.SandyBrown,
                            0,
                            Vector2.Zero,
                            1.0f, //scale
                            SpriteEffects.None,
                            depth * 0.00001f);
                    }
                }
            }

            Storage.SB.End();

            //set rendering back to the back buffer
            GraphicsDevice.SetRenderTarget(null);

            //render target to back buffer
            targetBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
            targetBatch.Draw(target, new Rectangle(0, 0, 512, 512), Color.White);
            targetBatch.End();

            base.Draw(gameTime);
        }
    }
}
