using Microsoft.Xna.Framework;
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
        public static List<Particle> particles;
        public static bool[,] grid;
    }

    public struct Particle
    {
        public int x, y;
        public int x_velocity, y_velocity;
        public Color color;
    }

    public class Game1 : Game
    {
        public Game1()
        {
            Storage.GDM = new GraphicsDeviceManager(this);
            Storage.GDM.GraphicsProfile = GraphicsProfile.HiDef;
            Storage.CM = Content;
            Storage.GAME = this;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            Storage.particles = new List<Particle>();
            Storage.GDM.PreferredBackBufferWidth = 512;
            Storage.GDM.PreferredBackBufferHeight = 512;
            Storage.GDM.ApplyChanges();
            Storage.grid = new bool[512, 512];

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

            for (int i = 0; i < Storage.particles.Count; i++)
            {
                var particle = Storage.particles[i];

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

                int next_x = particle.x + particle.x_velocity;
                int next_y = particle.y + particle.y_velocity;
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
                
                else if (!find_collision(next_x - 1, particle.y))
                {
                    next_x -= 1;
                    next_y = particle.y;
                    particle.y_velocity = 0;
                }
                else if (!find_collision(next_x + 1, particle.y))
                {
                    next_x += 1;
                    next_y = particle.y;
                    particle.y_velocity = 0;
                }
                
                else
                {
                    next_x = particle.x;
                    particle.x_velocity = 0;

                    next_y = particle.y;
                    particle.y_velocity = 0;
                }
                Storage.grid[particle.x, particle.y] = false;
                particle.x = next_x;
                particle.y = next_y;
                Storage.grid[next_x, next_y] = true;
                Storage.particles[i] = particle;
            }

            var mouse_state = Mouse.GetState();
            if (mouse_state.LeftButton == ButtonState.Pressed)
            {
                var x = mouse_state.Position.X * 64 / 512;
                var y = mouse_state.Position.Y * 64 / 512;
                Particle particle = new Particle();
                particle.x = x;
                particle.y = y;
                Storage.grid[x, y] = true;
                Storage.particles.Add(particle);
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
            // Use the grid to find if the spot is occupied.
            return Storage.grid[x, y];
            /*
            // Inefficient loop method for finding collisions.
            for (int i = 0; i < Storage.particles.Count; i++)
            {
                if (Storage.particles[i].x == x && Storage.particles[i].y == y)
                {
                    return true;
                }
            }
            return false;
            */
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

            for (int i = 0; i < Storage.particles.Count; i++)
            {
                Vector2 pos = new Vector2(Storage.particles[i].x, Storage.particles[i].y);
                //draw each particle as a sprite
                Storage.SB.Draw(Storage.texture,
                    pos,
                    Storage.drawRec,
                    Color.SandyBrown * 1,
                    0,
                    Vector2.Zero,
                    1.0f, //scale
                    SpriteEffects.None,
                    i * 0.00001f);
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
