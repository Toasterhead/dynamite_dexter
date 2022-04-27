using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input;

namespace DynamiteDexter
{
    public partial class Game1 : Game
    {
        public static bool IntersectsCircular(IGameObject left, IGameObject right)
        {
            double radiusLeft = left.Width / 2;
            double radiusRight = right.Width / 2;

            double a = right.Center.X - left.Center.X;
            double b = right.Center.Y - left.Center.Y;
            double c = Math.Sqrt(Math.Pow(a, 2) + Math.Pow(b, 2));

            return c < radiusLeft + radiusRight;
        }

        public bool IntersectsCircular(Rectangle left, Rectangle right)
        {
            //Note - Uses rectangle's width as the diameter. Ovals not supported.

            double radiusLeft = left.Width / 2;
            double radiusRight = right.Width / 2;

            double centerLeftX = (left.X + left.Width) - radiusLeft;
            double centerLeftY = (left.Y + left.Height) - (left.Height / 2);
            double centerRightX = (right.X + right.Width) - radiusRight;
            double centerRightY = (right.Y + right.Height) - (right.Height / 2);

            double a = centerRightX - centerLeftX;
            double b = centerRightY - centerLeftY;
            double c = Math.Sqrt(Math.Pow(a, 2) + Math.Pow(b, 2));

            return c < radiusLeft + radiusRight;
        }

        public static bool WorldCursorInRange()
        {
            return
                worldCursor.X >= 0 &&
                worldCursor.Y >= 0 &&
                worldCursor.X < WORLD_SIZE_X &&
                worldCursor.Y < WORLD_SIZE_Y;
        }

        public static Graph GenerateGraph(List<IGameObject> spriteSet, SnakeSegment snakeHead = null)
        {
            bool[,] mapGrid = new bool[GRID_SIZE_X, GRID_SIZE_Y];
            Graph graph = new Graph();

            for (int i = 0; i < GRID_SIZE_Y; i++)
                for (int j = 0; j < GRID_SIZE_X; j++)
                    mapGrid[j, i] = true;

            foreach (IGameObject i in spriteSet)
            {
                int gridX = i.X / TILE_SIZE;
                int gridY = i.Y / TILE_SIZE;

                bool isSnakeHead = (snakeHead == null ? false : i == snakeHead);

                if (((i is Terrain && !(i is Passable)) || (i is SnakeSegment && !isSnakeHead)) &&
                    gridX >= 0 &&
                    gridY >= 0 &&
                    gridX < GRID_SIZE_X &&
                    gridY < GRID_SIZE_Y)

                    mapGrid[gridX, gridY] = false;
            }

            for (int i = 0; i < GRID_SIZE_Y; i++)

                for (int j = 0; j < GRID_SIZE_X; j++)
                {
                    if (mapGrid[j, i])
                    { 
                        graph.AddVertex(new Point(j, i));

                        if (j > 0 && mapGrid[j - 1, i])
                            graph.AddEdge(graph.Size - 1, graph.Size - 2);

                        if (i > 0 && mapGrid[j, i - 1])
                        
                            //Beginning iteration at 'graph.Size - 2' to skip the current vertex.

                            for (int k = graph.Size - 2; k > 0; k--)
                            {
                                Point vertexData = graph.GetVertexData(k);

                                if (vertexData.X == j && vertexData.Y == i - 1)
                                {
                                    graph.AddEdge(graph.Size - 1, k);
                                    break;
                                }
                            }
                    }
                }

            return graph;
        }

        public static List<Point> GetSupportedResolutions()
        {
            List<Point> supportedResolutions = new List<Point>();

            foreach (DisplayMode displayMode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
            {
                bool duplicateFound = false;

                foreach (Point resolution in supportedResolutions)

                    if (displayMode.Width == resolution.X && displayMode.Height == resolution.Y)
                    {
                        duplicateFound = true;
                        break;
                    }

                if (!duplicateFound) supportedResolutions.Add(new Point(displayMode.Width, displayMode.Height));
            }

            return supportedResolutions;
        }

        public static bool IsOutdoor(Game1.Environments environment)
        {
            if (environment == Environments.Garden ||
                environment == Environments.Forest ||
                environment == Environments.Desert ||
                environment == Environments.Jungle ||
                environment == Environments.Cave ||
                environment == Environments.Mine)

                return true;

            return false;
        }

        public static void PlayMusic(Song song, bool repeat = false)
        {
            if (currentSong != song)
            { 
                MediaPlayer.Stop();
                MediaPlayer.Play(song);
                MediaPlayer.IsRepeating = repeat;
                currentSong = song;
            }
        }

        public static void StopMusic()
        {
            MediaPlayer.Stop();
            MediaPlayer.IsRepeating = false;
            currentSong = null;
        }

        public static void PlaySound(SoundEffectInstance sound)
        {
            if (currentSound != null && currentSound.State == SoundState.Playing)
            {
                Sounds.SoundCharacteristics targetCharacteristics = Sounds.GetCharacteristics(sound);
                Sounds.SoundCharacteristics currentCharacteristics = Sounds.GetCharacteristics(currentSound);

                if ((targetCharacteristics.interrupts && targetCharacteristics.priority < currentCharacteristics.priority) ||
                    (!targetCharacteristics.interrupts && targetCharacteristics.priority <= currentCharacteristics.priority))

                    return;
            }

            if (currentSound != null)
                currentSound.Stop();

            if (currentSound == sound)
            {
                SoundEffectInstance duplicateSound = Sounds.RetrieveDuplicate(sound);
                currentSound = duplicateSound == null ? sound : duplicateSound;
            }
            else currentSound = sound;

            currentSound.Volume = (float)MenuManager.TitleMenu.GetDoubleVal("soundVolume");
            currentSound.Play();
        }

        public static void ProcessQueuedSound()
        {
            if (queuedSound != null && currentSound.State == SoundState.Stopped)
            {
                PlaySound(queuedSound);
                queuedSound = null;
            }
        }

        public bool ButtonPressed(GamePadState gamepad, GamePadState gamepadPrev)
        {
            return
                (gamepad.Buttons.A == ButtonState.Pressed && gamepadPrev.Buttons.A == ButtonState.Released) ||
                (gamepad.Buttons.B == ButtonState.Pressed && gamepadPrev.Buttons.B == ButtonState.Released) ||
                (gamepad.Buttons.X == ButtonState.Pressed && gamepadPrev.Buttons.X == ButtonState.Released) ||
                (gamepad.Buttons.Y == ButtonState.Pressed && gamepadPrev.Buttons.Y == ButtonState.Released);
        }

        public bool DPadLeftPressed(GamePadState gamepad, GamePadState gamepadPrev)
        {
            return gamepad.DPad.Left == ButtonState.Pressed && gamepadPrev.DPad.Left == ButtonState.Released;
        }

        public bool DPadRightPressed(GamePadState gamepad, GamePadState gamepadPrev)
        {
            return gamepad.DPad.Right == ButtonState.Pressed && gamepadPrev.DPad.Right == ButtonState.Released;
        }

        public bool DPadUpPressed(GamePadState gamepad, GamePadState gamepadPrev)
        {
            return gamepad.DPad.Up == ButtonState.Pressed && gamepadPrev.DPad.Up == ButtonState.Released;
        }

        public bool DPadDownPressed(GamePadState gamepad, GamePadState gamepadPrev)
        {
            return gamepad.DPad.Down == ButtonState.Pressed && gamepadPrev.DPad.Down == ButtonState.Released;
        }
    }
}
