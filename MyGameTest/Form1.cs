using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;

using System.Windows.Forms;

namespace MyGameTest
{
    public partial class Game1 : Form
    {
        //Graphics object
        Graphics g;
        //Player cursor location
        Vector3 cursor = new Vector3(0, 0, 0);
        //Camera offset
        Vector2 camera = new Vector2(0, 0);
        int scrollSpeed = 10;

        int worldSize = 48;//48
        int worldHeight = 16;//16

        //Holds all the info of the game chunck/grid
        Cell[][][] worldGrid;
        
        int frame = 0;

        //game times that clears the screen and redraws it
        private void TimerCallback(object sender, EventArgs e)
        {
            //Just a makeshift frame rate counter. just dont go past 2 billion ;)
            frame++;
            //redraw the screen
            Invalidate();
            return;
        }

        //this runs before the programs starts
        public Game1()
        {
            worldGrid = new Cell[worldSize][][];
            for (int i = 0; i < worldSize; i++)
            {
                worldGrid[i] = new Cell[worldSize][];
                for (int j = 0; j < worldSize; j++)
                {
                    worldGrid[i][j] = new Cell[worldHeight];
                    for(int k = 0; k < worldHeight; k++)
                    {
                        worldGrid[i][j][k] = new Cell(BlockTypes.stone);
                    }
                }

            }

            var timer = new Timer();
            timer.Enabled = false;
            timer.Interval = 33;  /* 100 millisec */
            timer.Tick += new EventHandler(TimerCallback);
            timer.Start();

            InitializeComponent();
        }

        //This is what runs each frame to draw the game
        //Main game loop
        private void Game1_Paint(object sender, PaintEventArgs e)
        {
            g = e.Graphics;
            //Draws the world
            drawWorld();
            //Draws the frame rate
            g.DrawString(frame.ToString(), new Font("Arial", 12), new SolidBrush(Color.Red), 5, 5);
        }

        private void drawWorld()
        {
            
            for (int x = 0; x < worldSize; x++)
            {
                for (int y = 0; y < worldSize; y++)
                {
                    for (int z = 0; z < worldHeight; z++)
                    {
                        

                        //Occlusion culling
                        if(x + 1 < worldSize && y + 1 < worldSize && z + 1 < worldHeight)
                        {
                            //if the block in front is transparent then draw
                            if(worldGrid[x+1][y+1][z+1].blockType == BlockTypes.air)
                            {
                                drawBlock(x, y, z);                               
                            }
                        }
                        else
                        {
                            drawBlock(x, y, z);
                        }
                    }
                }
            }
            
        }

        //Draws an individual block at the given coordinates
       private void drawBlock(int x, int y, int z)
        {
            int screenX = y * 43 + -x * 43;
            int screenY = y * 25 + -x * -25 + z * -50;

            //1 oclock face
            g.FillPolygon(new SolidBrush(Color.LightGray), new PointF[] { new PointF(screenX - camera.x, screenY - camera.y), new PointF(screenX - camera.x, screenY - 50 - camera.y), new PointF(screenX + 43 - camera.x, screenY - 25 - camera.y) });

            //3 oclock face
            g.FillPolygon(new SolidBrush(Color.Gray), new PointF[] { new PointF(screenX - camera.x, screenY - camera.y), new PointF(screenX + 43 - camera.x, screenY - 25 - camera.y), new PointF(screenX + 43 - camera.x, screenY + 25 - camera.y) });

            //5 oclock face
            g.FillPolygon(new SolidBrush(Color.Gray), new PointF[] { new PointF(screenX - camera.x, screenY - camera.y), new PointF(screenX + 43 - camera.x, screenY + 25 - camera.y), new PointF(screenX - camera.x, screenY + 50 - camera.y) });

            //7 oclock face
            g.FillPolygon(new SolidBrush(Color.DarkGray), new PointF[] { new PointF(screenX - camera.x, screenY - camera.y), new PointF(screenX - 43 - camera.x, screenY + 25 - camera.y), new PointF(screenX - camera.x, screenY + 50 - camera.y) });

            //9 oclock face
            g.FillPolygon(new SolidBrush(Color.DarkGray), new PointF[] { new PointF(screenX - camera.x, screenY - camera.y), new PointF(screenX - 43 - camera.x, screenY + 25 - camera.y), new PointF(screenX - 43 - camera.x, screenY - 25 - camera.y) });

            //11 oclock face
            g.FillPolygon(new SolidBrush(Color.LightGray), new PointF[] { new PointF(screenX - camera.x, screenY - camera.y), new PointF(screenX - camera.x, screenY - 50 - camera.y), new PointF(screenX - 43 - camera.x, screenY - 25 - camera.y) });

        }

        //input handler
        //At a later date I will make this more robust and smoother
        private void Game1_KeyDown(object sender, KeyEventArgs e)
        {
            //w forward
            if(e.KeyValue == 87)
            {
                cursor.x -= 1;
            }
            //s backward
            else if (e.KeyValue == 83)
            {
                cursor.x += 1;
            }
            //a left
            else if (e.KeyValue == 65)
            {
                cursor.y -= 1;
            }
            //d right
            else if (e.KeyValue == 68)
            {
                cursor.y += 1;
            }
            //q up
            else if (e.KeyValue == 81)
            {
                cursor.z += 1;
            }
            //e down
            else if (e.KeyValue == 69)
            {
                cursor.z -= 1;
            }
            //camera up
            else if(e.KeyValue == 38)
            {
                camera.y = camera.y - scrollSpeed;
            }
            //camera down
            else if(e.KeyValue == 40)
            {
                camera.y = camera.y + scrollSpeed;
            }
            //camera left
            else if (e.KeyValue == 37)
            {
                camera.x = camera.x - scrollSpeed;
            }
            //camera right
            else if (e.KeyValue == 39)
            {
                camera.x = camera.x + scrollSpeed;
            }
        }
    }
}
