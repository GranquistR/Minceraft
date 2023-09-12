using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

using System.Windows.Forms;

namespace MyGameTest
{
    public partial class Game1 : Form
    {
        //Graphics object
        Graphics g;

        //world size
        int worldSize = 8;//48
        int worldHeight = 16;//16

        //Player cursor location
        Vector3 cursor = new Vector3(0, 0, 0);

        //Camera offset
        Vector2 camera = new Vector2(0, 0);
        int scrollSpeed = 10;


        //Holds all the info of the game chunck/grid
        Cell[][][] worldGrid;

        //frame counter
        int frame = 0;

        bool isDebugMode = false;

        //Inventory
        List<BlockTypes> inventory = new List<BlockTypes>();
        int selectedInventory = 1;

        //sprite sheet
        Bitmap blockSpriteSheet = new Bitmap(Image.FromFile(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\Textures\tinyBlocks.png")));
        Bitmap inventroySprite = new Bitmap(Image.FromFile(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\Textures\inventoryCell.png")));
        Bitmap selectedInventroySprite = new Bitmap(Image.FromFile(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\Textures\inventoryCellSelected.png")));

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
            //fills world with default blocks
            worldGrid = new Cell[worldSize][][];
            for (int i = 0; i < worldSize; i++)
            {
                worldGrid[i] = new Cell[worldSize][];
                for (int j = 0; j < worldSize; j++)
                {
                    worldGrid[i][j] = new Cell[worldHeight];
                    for(int k = 0; k < worldHeight; k++)
                    {
                        if(k < 3)
                        {
                            worldGrid[i][j][k] = new Cell(BlockTypes.stone);
                        }else if(k < 6)
                        {
                            worldGrid[i][j][k] = new Cell(BlockTypes.dirt);
                        }else if(k < 7)
                        {
                            worldGrid[i][j][k] = new Cell(BlockTypes.grass);
                        }
                        else
                        {
                            worldGrid[i][j][k] = new Cell(BlockTypes.air);
                        }
                        
                    }
                }

            }

            //frame timer setup
            var timer = new Timer();
            timer.Enabled = false;
            timer.Interval = 33;  /* 100 millisec */
            timer.Tick += new EventHandler(TimerCallback);
            timer.Start();

            //sets cursor to default location
            cursor = new Vector3(worldSize - 1, worldSize - 1, worldHeight - 1);

            //FIlls inventory with default blocks
            inventory.Add(BlockTypes.stone);
            inventory.Add(BlockTypes.dirt);
            inventory.Add(BlockTypes.grass);
            inventory.Add(BlockTypes.logs);
            inventory.Add(BlockTypes.leaves);
            inventory.Add(BlockTypes.planks);
            inventory.Add(BlockTypes.bricks);
            inventory.Add(BlockTypes.sand);
            inventory.Add(BlockTypes.cobble);
            inventory.Add(BlockTypes.mossyCobble);
            inventory.Add(BlockTypes.air);

            InitializeComponent();
        }

        //This is what runs each frame to draw the game
        //Main game loop
        private void Game1_Paint(object sender, PaintEventArgs e)
        {
            //Sets up the graphics object and determines how it will draw
            g = e.Graphics;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;


            //Draws the world
            drawWorld();

            //Draws inventory
            for(int i = 0; i < inventory.Count(); i++)
            {
                var spriteLoc = GetSpriteCoord(inventory[i]);
                if (selectedInventory == i)
                {
                    g.DrawImage(selectedInventroySprite, new Rectangle(80 * i, 0, 80, 80));
                    g.DrawImage(blockSpriteSheet, new Rectangle(80 * i + 15, 15, 50, 50), new Rectangle(1 + (18 * spriteLoc.x), (18 * spriteLoc.y), 16, 18), GraphicsUnit.Pixel);
                }
                else
                {
                    g.DrawImage(inventroySprite, new Rectangle(80 * i, 0, 80, 80));
                    g.DrawImage(blockSpriteSheet, new Rectangle(80 * i + 15, 15, 50, 50), new Rectangle(1 + (18 * spriteLoc.x), (18 * spriteLoc.y), 16, 18), GraphicsUnit.Pixel);
                }
            }

            #region debug tools
            if (isDebugMode)
            {
                //Draws the frame rate
                g.DrawString(frame.ToString(), new Font("Arial", 12), new SolidBrush(Color.Red), 5, 5);
                //draws the cursor location
                g.DrawString($"Cursor Location: ({cursor.x},{cursor.y},{cursor.z})", new Font("Arial", 12), new SolidBrush(Color.Red), 5, 20);
            }
            #endregion
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
                                drawBlock(x, y, z, worldGrid[x][y][z].blockType);
                                if (x == cursor.x && y == cursor.y && z == cursor.z)
                                {
                                    drawBlock(x, y, z, BlockTypes.cursor);
                                }
                            }
                        }
                        else
                        {
                            drawBlock(x, y, z, worldGrid[x][y][z].blockType);
                            if (x == cursor.x && y == cursor.y && z == cursor.z)
                            {
                                drawBlock(x, y, z, BlockTypes.cursor);
                            }
                        }
                    }
                }
            }
            
        }

        //Draws an individual block at the given coordinates
       private void drawBlock(int x, int y, int z, BlockTypes type)
        {
            int screenX = y * 43 + -x * 44;
            int screenY = y * 22 + -x * -22 + z * -50;

            //find sprite offset
            var spriteLoc = GetSpriteCoord(type);

            g.DrawImage(blockSpriteSheet, new Rectangle(screenX - camera.x, screenY - camera.y,87,100), new Rectangle(1 + (18 * spriteLoc.x), (18 * spriteLoc.y), 16, 18), GraphicsUnit.Pixel);
        }

        private Vector2 GetSpriteCoord(BlockTypes type)
        {
            int spriteX = 0;
            int spriteY = 0;
            if (type == BlockTypes.stone)
            {
                spriteX = 0;
                spriteY = 5;
            }
            else if (type == BlockTypes.logs)
            {
                spriteX = 0;
                spriteY = 3;
            }
            else if (type == BlockTypes.grass)
            {
                spriteX = 0;
                spriteY = 1;
            }
            else if (type == BlockTypes.leaves)
            {
                spriteX = 2;
                spriteY = 4;
            }
            else if (type == BlockTypes.dirt)
            {
                spriteX = 6;
                spriteY = 0;
            }
            else if (type == BlockTypes.cursor)
            {
                spriteX = 8;
                spriteY = 7;
            }
            else if (type == BlockTypes.planks)
            {
                spriteX = 4;
                spriteY = 3;
            }
            else if (type == BlockTypes.bricks)
            {
                spriteX = 2;
                spriteY = 6;
            }
            else if (type == BlockTypes.sand)
            {
                spriteX = 4;
                spriteY = 0;
            }
            else if (type == BlockTypes.cobble)
            {
                spriteX = 4;
                spriteY = 6;
            }
            else if (type == BlockTypes.mossyCobble)
            {
                spriteX = 0;
                spriteY = 8;
            }
            else if (type == BlockTypes.air)
            {
                spriteX = -1;
                spriteY = -1;
            }
            else
            {
                spriteX = -1;
                spriteY = -1;
            }
            return new Vector2(spriteX, spriteY);
        }

        //input handler
        //At a later date I will make this more robust and smoother
        private void Game1_KeyDown(object sender, KeyEventArgs e)
        {
            #region cursor control
            //w forward
            if (e.KeyValue == 87)
            {
                if (cursor.x > 0)
                {
                    cursor.x -= 1;
                }
            }
            //s backward
            else if (e.KeyValue == 83)
            {
                if (cursor.x < worldSize - 1)
                {
                    cursor.x += 1;
                }
            }
            //a left
            else if (e.KeyValue == 65)
            {
                if (cursor.y > 0)
                {
                    cursor.y -= 1;
                }
            }
            //d right
            else if (e.KeyValue == 68)
            {
                if (cursor.y < worldSize - 1)
                {
                    cursor.y += 1;
                }
            }
            //q up
            else if (e.KeyValue == 81)
            {
                if (cursor.z < worldHeight - 1)
                {
                    cursor.z += 1;
                }
            }
            //e down
            else if (e.KeyValue == 69)
            {
                if (cursor.z > 0)
                {
                    cursor.z -= 1;
                }
            }
            else if (e.KeyValue == 32)
            {
                worldGrid[cursor.x][cursor.y][cursor.z].blockType = inventory[selectedInventory];
            }
            #endregion
            #region camera control
            //camera up
            else if (e.KeyValue == 38)
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
            //enter debug mode
            else if (e.KeyValue == 86)
            {
                isDebugMode = !isDebugMode;
            }
            #endregion
            #region inventory control
            else if (e.KeyValue == 188)
            {
                if (selectedInventory > 0)
                {
                    selectedInventory--;
                }
            }
            else if (e.KeyValue == 190)
            {
                if(selectedInventory < inventory.Count - 1)
                {
                    selectedInventory++;
                }
            }
            #endregion
        }
    }
}
