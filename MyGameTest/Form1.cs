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
        Graphics g;
        Vector3 cursor = new Vector3(0, 0, 0);
        Vector2 camera = new Vector2(0, 0);

        int worldSize = 8;
        int worldHeight = 8;
        Cell[][][] worldGrid;

        private void TimerCallback(object sender, EventArgs e)
        {
            //redraw the screen
            Invalidate();
            return;
        }
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
        private void Game1_Paint(object sender, PaintEventArgs e)
        {
            g = e.Graphics;

            drawWorld();
            
        }

        private void drawWorld()
        {
            for (int i = 0; i < worldSize; i++)
            {
                for (int j = 0; j < worldSize; j++)
                {
                    for (int k = 0; k < worldHeight; k++)
                    {
                        drawBox(new Vector3(i, j, k), worldGrid[i][j][k]);
                    }
                }
            }
        }

        private void drawBox(Vector3 coord, Cell cell)
        {
            Pen p = new Pen(Color.White);
            Brush bTop = new SolidBrush(Color.LightGray);
            Brush bLeft = new SolidBrush(Color.DarkGray);
            Brush bRight = new SolidBrush(Color.DimGray);

            int screenX = coord.y * 43 + -coord.x * 43;
            int screenY = coord.y * 25 + -coord.x * -25 + coord.z * -50;

            //left face
            //Top
            g.FillPolygon(bLeft, new PointF[] { new PointF(screenX - camera.x, screenY - camera.y), new PointF(screenX - 43 - camera.x, screenY + 25 - camera.y), new PointF(screenX - 43 - camera.x, screenY - 25 - camera.y) });
            //Bottom
            g.FillPolygon(bLeft, new PointF[] { new PointF(screenX - camera.x, screenY - camera.y), new PointF(screenX - 43 - camera.x, screenY + 25 - camera.y), new PointF(screenX - camera.x, screenY + 50 - camera.y) });

            //right face
            //top
            g.FillPolygon(bRight, new PointF[] { new PointF(screenX - camera.x, screenY - camera.y), new PointF(screenX + 43 - camera.x, screenY - 25 - camera.y), new PointF(screenX + 43 - camera.x, screenY + 25 - camera.y) });
            //bottom
            g.FillPolygon(bRight, new PointF[] { new PointF(screenX - camera.x, screenY - camera.y), new PointF(screenX + 43 - camera.x, screenY + 25 - camera.y), new PointF(screenX - camera.x, screenY + 50 - camera.y) });

            //top face
            //right
            g.FillPolygon(bTop, new PointF[] { new PointF(screenX - camera.x, screenY - camera.y), new PointF(screenX - camera.x, screenY - 50 - camera.y), new PointF(screenX + 43 - camera.x, screenY - 25 - camera.y) });
            //left
            g.FillPolygon(bTop, new PointF[] { new PointF(screenX - camera.x, screenY - camera.y), new PointF(screenX - camera.x, screenY - 50 - camera.y), new PointF(screenX - 43 - camera.x, screenY - 25 - camera.y) });

        }

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
                camera.y = camera.y - 3;
            }
            //camera down
            else if(e.KeyValue == 40)
            {
                camera.y = camera.y + 3;
            }
            //camera left
            else if (e.KeyValue == 37)
            {
                camera.x = camera.x - 3;
            }
            //camera right
            else if (e.KeyValue == 39)
            {
                camera.x = camera.x + 3;
            }
        }
    }
}
