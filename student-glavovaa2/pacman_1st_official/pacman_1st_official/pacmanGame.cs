using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pacman_1st_official
{
    public partial class pacmanGame : Form
    {
        
        private bool startDelay, endDelay;
        private Pacman player;
        private Ghost blueG;

        public pacmanGame()
        {
            InitializeComponent();

            PacmanMaps.InitializeGrid(PacmanMaps.getCurrentMap());
            player = new Pacman();
            blueG = new Ghost(Color.Blue);

            Coin.getNumOfCoins(this.Controls);

            restartGame();
        }


        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {
            
        }

        private void delayCounter_Tick(object sender, EventArgs e)
        {
            if (!startDelay)
            {
                startDelay = true;
            }
            else
            {
                endDelay = true;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void pacman_Click(object sender, EventArgs e)
        {

        }

        private void gameTimer_Tick(object sender, EventArgs e)
        {
            // Spustí sa pri každom tiknutí timeru (cca. každých 20ms)
            // Predá referencie triede Pacman. Tá skontroluje, ktoré klávesy sú stlačené a či sa pacman môže posunúť. 
            // Zároveň zmení obrázok (pri zmene smeru/ keď zatvára ústa)
            // Na konci sa pokúsi vyzbierať coiny, ak na nich pacman stojí a skontroluje, či sa hra neskončila.
            
            int top = pacman.Top;
            int left = pacman.Left;
            System.Drawing.Bitmap image = Properties.Resources.pacmanClosedMouth;

            player.pacmanMovement(ref startDelay, ref endDelay, pacman.Location.X, pacman.Location.Y, ref top, ref left, ref image);

            pacman.Left = left;
            pacman.Top = top;
            pacman.Image = image;

            if (collisionPacmanGhost())
            {
                // Ak pacman narazí do ghosta tak sa vráti na pôvodné súradnice a odpočíta sa mu život
                int[] restorePacmanCoords = PacmanMaps.getStartingCoords();
                pacman.Left = restorePacmanCoords[0];
                pacman.Top = restorePacmanCoords[1];
                Pacman.removeLife();
            }

            label1.Text = optnsForm.gameDifficulty.ToString();

            checkForCoins();
            gameEnd();

        }

        private bool collisionPacmanGhost()
        {
            // Skontroluje, či sa súradnice pacmana prekrývajú so súradnicami ghosta
            foreach (Control p in this.Controls)
            {
                
                if (p is PictureBox && (string)p.Tag == "ghost" && pacman.Bounds.IntersectsWith(p.Bounds))
                {
                    return true;
                }
            }
            return false;
        }

        private void gameEnd()
        {
            // Hra končí, ak sú pozbierané všetky coiny
            // pridať že ghosti sa nebudú hýbať
            if (Coin.allCoinsCollected())
            {
                txtWin.Text = "VYHRA :)";
                txtWin.BackColor = Color.LawnGreen;
                txtWin.Visible = true;
                gameTimer.Stop();
            }

            if (Pacman.livesLeft() == 0)
            {
                txtWin.Text = "PREHRA :(";
                txtWin.BackColor = Color.Red;
                txtWin.Visible = true;
                gameTimer.Stop();
            }

           
        }
        private void checkForCoins()
        {
            // Skontroluje, či sa poloha pacmana prekrýva s PictureBoxom, ktorý má tag "coin"
            // Ak áno, tak pripočíta skóre, updatne text a vyplne viditeľnosť coinu 
            foreach (Control c in this.Controls)
            {
                if (c is PictureBox && (string)c.Tag == "coin" && c.Visible)
                {
                    if (pacman.Bounds.IntersectsWith(c.Bounds))
                    {
                        c.Visible = false;
                        txtScore.Text = Coin.updateScore();
                    }
                }
            }
        }


        private void pictureBox5_Click_1(object sender, EventArgs e)
        {

        }

        private void pictureBox10_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox11_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox12_Click(object sender, EventArgs e)
        {

        }

        private void pacmanGame_KeyDown(object sender, KeyEventArgs e)
        {
            // Klávesa zapnutá -- nastaví bool na true
            player.manageKeys(true, e);
        }
        private void pacmanGame_KeyUp(object sender, KeyEventArgs e)
        {
            // Klávesa vypnutá -- bool nastaví na false
            player.manageKeys(false,e);
        }

        private void ghostPathTimer_Tick(object sender, EventArgs e)
        {
            label1.Text = blueG.findGhostsPosition(blueGhost.Location.X, blueGhost.Location.Y);
            label2.Text = blueG.findPacmansPosition(pacman.Location.X, pacman.Location.Y);
            label3.Text = blueG.findShortestPath();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void restartGame()
        {
            // Spustí sa na začiatku hry a po každej prehre
            // Spustí timery, reštartuje skóre, ...
            // Delay counter slúži na otváranie a zatváranie úst pacmana
            gameTimer.Start();
            delayCounter.Start();

            txtScore.Text = "Score: 0";
            txtWin.Visible = false;

            Coin.resetScore();
            Pacman.restoreLives();

            // Vráti pacmana na pôvodné súradnice
            int[] restorePacmanCoords = PacmanMaps.getStartingCoords();
            pacman.Left = restorePacmanCoords[0];
            pacman.Top = restorePacmanCoords[1];

            // Všetkým coinom naspäť nastaví viditeľnosť na true
            foreach (Control c in this.Controls)
            {
                if (c is PictureBox && (string)c.Tag == "coin" )
                {
                    c.Visible = true;
                }
            }
        }
    }

    public static class PacmanMaps
    {
        /// <summary>
        /// 
        /// Uchováva informácie o jednotlivých mapách -- počet riadkov/stĺpcov, veľkosť bunky, veľkosť celej obrazovky
        /// Layout mapy je reprezentovaný 0 a 1 -- 0 znamená stena, 1 znamená chodba.
        /// Na základe layoutu mapy inicializuje grid, podľa ktorého sa potom detekujú steny.
        /// 
        /// </summary>
        
        private const int numRows = 10;
        public const int numCols = 10;
        public const int windowSize = 360;
        public const int cellSize = 36;

        private static string[] listOfMaps = { mapLvl1, mapLvl2, mapLvl3 };

        private static int currentMap = optnsForm.level - 1;

        public static int[,] pacmanStartingCoords = new int[3, 2]
        {
            { 80, 300 },
            { 1, 0 },
            { 2, 0 },
        };

        static List<int[]> walls = new List<int[]>();

        public const string mapLvl1 = 
            "0000000000" +
            "0111001110" +
            "0111111110" +
            "0010110100" +
            "1110110111" +
            "0010000100" +
            "0111111110" +
            "0100110010" +
            "0110110110" +
            "0000000000";

        public static string mapLvl2 = 
            "0000000000" +
            "0111111110" +
            "0100101010" +
            "0111101110" +
            "1100111011" +
            "1101101011" +
            "0111011110" +
            "0010110010" +
            "0111101110" +
            "0000000000";

        public static string mapLvl3 = 
            "0000000000" +
            "0111111110" +
            "0100110010" +
            "0110110110" +
            "0010110100" +
            "0110000110" +
            "0100111010" +
            "0110100110" +
            "0011111100" +
            "0000000000";

        public static void InitializeGrid(string map)
        {
            // vytvorí List<int[]> na základe stringu núl a jednotiek, reprezentujúcich mapu
            // List reprezentuje grid -- steny majú X a Y koordináciu
            int index = 0;

            for (int row = 0; row < numRows; row++)
            {
                for (int col = 0; col < numCols; col++)
                {

                    if (isWall(map[index]))
                    {
                        int[] wall = new int[] { row, col };
                        walls.Add(wall);
                    }
                    index++;
                }
            }
        }

        private static bool isWall(char c)
        {
            // 0 = je stena; 1 = nie je stena
            return (c == '0');
        }

        public static string getCurrentMap()
        {
            // Vráti layout aktuálnej mapy
            return listOfMaps[currentMap];
        } 

        public static List<int[]> getWalls()
        {
            return walls;
        }

        public static int[] getStartingCoords()
        {
            int[] temp = new int[2] { pacmanStartingCoords[currentMap, 0], pacmanStartingCoords[currentMap, 1] };
            return temp;
        }


    }

    public class Pacman
    {
        /// <summary>
        /// 
        /// Reprezentuje v hre samotného pacmana. Pacman si pamätá o sebe informácie ako rýchlosť, veľkosť a počet životov. 
        /// Rýchlosť a počet životov závisí od nastavení v optnsForm -- v OPTIONS tabe do ktorého sa dá dostať z hlavného menu.
        /// Životy si uberá na základe podnetov z hlavnej triedy. Vie ich vymazať alebo vrátiť do pôvodného stavu.
        /// Zároveň pozná svoj stav, tj. na ktorý smer je natočený. Vďaka tomu v metóde canMove kontroluje, či sa môže pohnúť bez narazenia do steny.
        /// 
        /// </summary>
        
        public bool goLeft, goRight, goUp, goDown;
        private int pacmanSpeed;

        private const int pacmanSize = 22;
        private static int lives;
        public Pacman()
        {
            pacmanSpeed = optnsForm.playerSpeedVal;

            // Na hard difficulty iba 1 život
            if (optnsForm.gameDifficulty == true)
            {
                lives = 3;
            }
            else
            {
                lives = 1;
            }
        }

        private bool canMove(int playerPosX, int playerPosY, int potentialIncX, int potentialIncY)
        {
            // Kontroluje, či sa po pohybe bude dotýkať nejakej steny
            // (tj. či vzdialenosť medzi pacmanom a stenou bude menšia než offset)
            foreach (int[] wall in PacmanMaps.getWalls())
            {

                int differenceX = Math.Abs(((wall[1] * PacmanMaps.cellSize) - playerPosX - potentialIncX));
                int differenceY = Math.Abs(((wall[0] * PacmanMaps.cellSize) - playerPosY - potentialIncY));
                int offset = pacmanSize;

                if (differenceX < offset && differenceY < offset)
                {
                    return false;
                }
            }
            return true;

        }

        public void pacmanMovement(ref bool startDelay, ref bool endDelay, int locationX, int locationY, ref int top, ref int left, ref System.Drawing.Bitmap image)
        {
            // Delay trvá 200ms a slúži na prepínanie obrázkov pacmana (otváranie a zatváranie úst). Delay počíta delayCounter -- tj. timer, ktorý tikne každých 200ms.
            if (endDelay)
            {
                startDelay = false;
                endDelay = false;
            }

            // Pre každý pohyb skontroluje, či by pacman nenarazil do steny kebyže sa pohne
            // Ak nie, tak ho posunie a aktualizuje obrázok (smer, kam sa pacman pozerá + či má otvorené/zavreté ústa)
            if (goLeft && canMove(locationX, locationY, -pacmanSpeed, 0))
            {
                left -= pacmanSpeed;
                image = startDelay && !endDelay ? Properties.Resources.pacmanClosedMouth : Properties.Resources.pacmanLeft;
            }
            else if (goRight && canMove(locationX, locationY, pacmanSpeed, 0))
            {
                left += pacmanSpeed;
                image = startDelay && !endDelay ? Properties.Resources.pacmanClosedMouth : Properties.Resources.pacmanRight;
            }
            else if (goUp && canMove(locationX, locationY, 0, -pacmanSpeed))
            {
                top -= pacmanSpeed;
                image = startDelay && !endDelay ? Properties.Resources.pacmanClosedMouth : Properties.Resources.pacmanUp;
            }
            else if (goDown && canMove(locationX, locationY, 0, pacmanSpeed))
            {
                top += pacmanSpeed;
                image = startDelay && !endDelay ? Properties.Resources.pacmanClosedMouth : Properties.Resources.pacmanDown;
            }

            // Teleport ak pacman odíde z obrazovky cez tunel
            if (left < -10)
            {
                left = PacmanMaps.windowSize;
            }
            else if (left > PacmanMaps.windowSize)
            {
                left = -10;
            }
        }

        public void manageKeys(bool upDown, KeyEventArgs e)
        {
            // Nastavuje booleany na true/false na základe toho, ktorá klávesa je stlačená
            switch (e.KeyCode)
            {
                case (Keys.Left):
                    goLeft = upDown;
                    break;
                case (Keys.Right):
                    goRight = upDown;
                    break;
                case (Keys.Up):
                    goUp = upDown;
                    break;
                case (Keys.Down):
                    goDown = upDown;
                    break;
            }
        }

        public static void restoreLives()
        {
            if (optnsForm.gameDifficulty)
            {
                lives = 3;
            }
            else
            {
                lives = 1;
            }
        }

        public static void removeLife()
        {
            lives--;
        }

        public static int livesLeft()
        {
            return lives;
        }
    }

    public static class Coin
    {
        /// <summary>
        /// 
        /// Statická trieda, reprezentuje coiny a skóre v hre.
        /// Pamätá si počet coinov, ktoré v hre ostávajú -- keď je tento počet 0, tak na to upozorní hlavný program.
        /// Pamätá si coiny, ktoré sme už vyzbierali -- na základe nich zvyšuje skóre. Ak sa hra reštartuje, tak skóre vymaže
        /// 
        /// </summary>
        
        private static int allCoins;
        public static int remainingCoins;
        private static int score;
        

        public static void getNumOfCoins(Control.ControlCollection controls)
        {
            int count = 0;

            foreach (Control control in controls)
            {
                if (control is PictureBox pictureBox && pictureBox.Tag != null)
                {
                    if (pictureBox.Tag.ToString() == "coin")
                    {
                        count++;
                    }
                }
            }

            allCoins = count;
            remainingCoins = allCoins;
        }

        public static string updateScore()
        {
            remainingCoins--;
            score++;
            return "Score: " + score.ToString();
        }

        public static bool allCoinsCollected()
        {
            // Používa sa pri kontrole, či hra už skončila
            return (remainingCoins == 0);
        }

        public static void resetScore()
        {
            score = 0;
            remainingCoins = allCoins;
        }
    }

    public class Ghost
    {
        System.Drawing.Color color;
        List<int[]> walls= new List<int[]>();
        private int[] pacmansPosition = new int[2];
        private int[] ghostsPosition = new int[2];
        private int ghostSpeed = 6;

        private List<int[]> currentShortestPath = new List<int[]>();
        private int curPathPointer = 0;

        public Ghost(System.Drawing.Color color) 
        {
            this.color = color;
            walls = PacmanMaps.getWalls();

        }

        public string findPacmansPosition(int x, int y)
        {
            x = (x + 15) / PacmanMaps.cellSize;
            y = (y + 15) / PacmanMaps.cellSize;

            pacmansPosition = new int[2] { x,y };
            return x.ToString() + "," + y.ToString();
        }

        public string findGhostsPosition(int x, int y)
        {
            x = (x + 15) / PacmanMaps.cellSize;
            y = (y + 15) / PacmanMaps.cellSize;

            ghostsPosition = new int[2] { x, y };
            return x.ToString() + "," + y.ToString();
        }

        /*public int followCurrentShortestPath()
        {
            int[] curCiel = currentShortestPath[curPathPointer];

            if ((ghostsPosition[0] - curCiel[0]) * 37 == 0 && (ghostsPosition[1] - curCiel[0]) * 37 == 0)
            {
                // už som tam
                curPathPointer++;
                curPathPointer = curPathPointer % 10;
            }

            int i = 0;
            if ((ghostsPosition[0] - curCiel[0]) * 37 > 0){
                // posuň doľava
                i = 1;
            }
            else if ((ghostsPosition[0] - curCiel[0]) * 37 < 0)
            {
                // posúň doprava
                i = 2;
            }

            if ((ghostsPosition[1] - curCiel[0]) * 37 > 0)
            {
                // posuň hore
                i = 3;
            }
            else if ((ghostsPosition[1] - curCiel[0]) * 37 < 0)
            {
                // posúň dole
                i = 4;
            }
            return i;
        }*/
        public string findShortestPath()
        {
            int[,] grid = new int[PacmanMaps.numCols, PacmanMaps.numCols];
            for (int i = 0; i < PacmanMaps.numCols; i++)
            {
                for (int j = 0; j < PacmanMaps.numCols; j++)
                {
                    grid[i, j] = -1;
                }
            }

            foreach (int[] wall in walls)
            {
                grid[wall[1], wall[0]] = -2;
            }

            int[,] possibleMoves = { { 0, 1 }, { 1, 0 }, { 0, -1 }, { -1, 0 } };

            Queue<int[]> q = new Queue<int[]>();
            int[] startingCoords = { ghostsPosition[0], ghostsPosition[1], 0 };
            q.Enqueue(startingCoords);

            int[,] parent = new int[10, 10];
            parent[ghostsPosition[0], ghostsPosition[1]] = -1;

            int result = -1;
            while (q.Count > 0)
            {
                int[] current = q.Dequeue();
                int curx = current[0];
                int cury = current[1];
                int curLen = current[2];

                if (curx == pacmansPosition[0] && cury == pacmansPosition[1])
                {
                    result = grid[curx, cury];
                    break;
                }

                for (int i = 0; i < 4; i++)
                {
                    int possibleX = current[0] + possibleMoves[i, 0];
                    int possibleY = current[1] + possibleMoves[i, 1];

                    if (possibleX >= 0 && possibleX < 10 && possibleY >= 0 && possibleY < 10 && grid[possibleX, possibleY] == -1 &&
                        grid[possibleX, possibleY] != -2)
                    {
                        int[] newVer = { possibleX, possibleY, curLen + 1 };
                        q.Enqueue(newVer);
                        parent[possibleX, possibleY] = curx * 8 + cury;

                        grid[possibleX, possibleY] = curLen + 1;
                    }
                }
            }

            if (result == -1)
            {
                Console.WriteLine(result);
                return result.ToString();
            }

            currentShortestPath = new List<int[]>();

            int x = pacmansPosition[0], y = pacmansPosition[1];

            while (x != -1 && y != -1)
            {
                int[] coords = { x, y };
                currentShortestPath.Insert(0, coords);
                int p = parent[x, y];
                x = p / 8;
                y = p % 8;
            }

            /*foreach (var coords in currentShortestPath)
            {
                Console.WriteLine($"{coords[0] + 1} {coords[1] + 1}");
            }*/

            return result.ToString();
        }
    }
}
