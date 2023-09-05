using System;
using System.CodeDom.Compiler;
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
        /// <summary>
        /// 
        /// Hlavná trieda, spája funkcie všetkých tried dokopy.
        /// Pri inicializácii vytvorí grid, ktorý potom reprezentuje mapu. 
        /// Vytvorí inštancie hráča a ghostov. Zaznamenáva klávesový input hráča a pomocou timeru posúva pacmana po mape. Druhý timer slúži na pohyb ghostov a tretí na prepočítavanie najkratšej cesty.
        /// Spúšťa, konroluje a ukončuje hru. Dokáže ju reštartovať, pri reštarte sa vynuluje skóre, hráč a ghosti sa vrátia na svoje pozície a regenerujú sa životy.
        /// 
        /// </summary>
        
        private bool startDelay, endDelay;
        private Pacman player;
        private Ghost blueG;
        private Ghost pinkG;

        public pacmanGame()
        {
            InitializeComponent();

            //pictureBox1.Image = PacmanMaps.uploadMapImage();
            PacmanMaps.InitializeGrid();
            Coin.getNumOfCoins(this.Controls);

            player = new Pacman();
            txtLivesLeft.Text = Pacman.livesLeft().ToString();
            blueG = new Ghost(Color.Blue, blueGhost.Location.X, blueGhost.Location.Y, pacman.Location.X, pacman.Location.Y);
            pinkG = new Ghost(Color.Pink, pinkGhost.Location.X, pinkGhost.Location.Y, pacman.Location.X, pacman.Location.Y);

            restartGame();

        }

        private bool collisionPacmanGhost()
        {
            // Skontroluje, či sa súradnice pacmana prekrývajú so súradnicami ghosta
            // (tj. či s pacmanom neintersectuje picturebox s tagom "ghost")
            foreach (Control p in this.Controls)
            {

                if (p is PictureBox && (string)p.Tag == "ghost" && pacman.Bounds.IntersectsWith(p.Bounds))
                {
                    return true;
                }
            }
            return false;
        }
        private void pacmanGame_KeyDown(object sender, KeyEventArgs e)
        {
            // Klávesa zapnutá -- nastaví bool na true
            player.manageKeys(true, e);
        }
        private void pacmanGame_KeyUp(object sender, KeyEventArgs e)
        {
            // Klávesa vypnutá -- bool nastaví na false
            player.manageKeys(false, e);
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
                int[] restorePacmanCoords = PacmanMaps.getStartingCoords('p');
                pacman.Left = restorePacmanCoords[0];
                pacman.Top = restorePacmanCoords[1];

                int[] restoreBlueGhostCoords = PacmanMaps.getStartingCoords('b');
                blueGhost.Left = restoreBlueGhostCoords[0];
                blueGhost.Top = restoreBlueGhostCoords[1];
                blueG.urobVsetkoCoSaDa(blueGhost.Location.X, blueGhost.Location.Y, pacman.Location.X, pacman.Location.Y);

                int[] restorePinkGhostCoords = PacmanMaps.getStartingCoords('k');
                pinkGhost.Left = restorePinkGhostCoords[0];
                pinkGhost.Top = restorePinkGhostCoords[1];
                pinkG.urobVsetkoCoSaDa(blueGhost.Location.X, blueGhost.Location.Y, pacman.Location.X, pacman.Location.Y);

                Pacman.removeLife();
            }

            txtLivesLeft.Text = "LIVES: " + Pacman.livesLeft().ToString(); ;

            checkForCoins();
            gameEnd();
        }

        private void ghostPathTimer_Tick(object sender, EventArgs e)
        {
            // Pri každom tiknutí sa ghost posunie alebo alignne.

            // Skontroluje, či je ghost správne alignnutý (pri prepočítaní cesty sa môže vykolajiť
            if (!blueG.isAlinged())
            {
                int[] temp = blueG.align(blueG.shortestPathQueue.Peek());
                blueGhost.Left -= temp[0];
                blueGhost.Top -= temp[1];

                return;
            }

            // Z fronty vezme najstarší príkaz a ten prevedie
            char command = blueG.dequeue();

            switch (command)
            {
                case 'r':
                    blueGhost.Left += blueG.ghostSpeed;
                    break;
                case 'l':
                    blueGhost.Left -= blueG.ghostSpeed;
                    break;
                case 'u':
                    blueGhost.Top += blueG.ghostSpeed;
                    break;
                case 'd':
                    blueGhost.Top -= blueG.ghostSpeed;
                    break;
                case 'e': // queue is empty.. waiting
                    break;
            }
        }

        private void pinkGhostT_Tick(object sender, EventArgs e)
        {
            if (!pinkG.isAlinged())
            {
                int[] temp = pinkG.align(pinkG.shortestPathQueue.Peek());
                pinkGhost.Left -= temp[0];
                pinkGhost.Top -= temp[1];

                return;
            }

            // Z fronty vezme najstarší príkaz a ten prevedie
            char command = pinkG.dequeue();

            switch (command)
            {
                case 'r':
                    pinkGhost.Left += pinkG.ghostSpeed;
                    break;
                case 'l':
                    pinkGhost.Left -= pinkG.ghostSpeed;
                    break;
                case 'u':
                    pinkGhost.Top += pinkG.ghostSpeed;
                    break;
                case 'd':
                    pinkGhost.Top -= pinkG.ghostSpeed;
                    break;
                case 'e': // queue is empty.. waiting
                    break;
            }
        }


        private void ghostCalculateNewPath_Tick(object sender, EventArgs e)
        {
            // Tikne raz za 2000ms, pri tom sa prepočíta nová cesta a pripraví všetko tak, aby ghost mohol ísť za pacmanom
            blueG.urobVsetkoCoSaDa(blueGhost.Location.X, blueGhost.Location.Y, pacman.Location.X, pacman.Location.Y);
            pinkG.urobVsetkoCoSaDa(pinkGhost.Location.X, pinkGhost.Location.Y, pacman.Location.X, pacman.Location.Y);

            /*pinkG.findGhostsPosition(pinkGhost.Location.X, pinkGhost.Location.Y); (Kebyže chcem pustiť blue ghosta na BFS a pink na dijkstre)
            pinkG.calculateMissalignment(pinkGhost.Location.X, pinkGhost.Location.Y);
            pinkG.findPacmansPosition(pacman.Location.X, pacman.Location.Y);
            pinkG.findShortestPathDijkstra();
            pinkG.prepareQueue();*/
        }

        private void delayCounter_Tick(object sender, EventArgs e)
        {
            // Delay slúži na prepínanie obrázku pacmana s otvorenými a zatvorenými ústami
            if (!startDelay)
            {
                startDelay = true;
            }
            else
            {
                endDelay = true;
            }
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
                ghostPathTimer.Stop();
                pinkGhostT.Stop();
            }

            if (Pacman.livesLeft() == 0)
            {
                txtWin.Text = "PREHRA :(";
                txtWin.BackColor = Color.Red;
                txtWin.Visible = true;
                gameTimer.Stop();
                ghostPathTimer.Stop();
                pinkGhostT.Stop();
            }
        }

        void restorePacmanPosition()
        {
            // Vráti pacmana na pôvodné súradnice
            int[] restorePacmanCoords = PacmanMaps.getStartingCoords('p');
            pacman.Left = restorePacmanCoords[0];
            pacman.Top = restorePacmanCoords[1];
        }

        private void restartGame()
        {
            // Spustí sa na začiatku hry a po každej prehre
            // Spustí timery, reštartuje skóre, ...
            // Delay counter slúži na otváranie a zatváranie úst pacmana

            txtScore.Text = "Score: 0";
            txtWin.Visible = false;

            resetTimers();
            Coin.resetScore();
            Pacman.restoreLives();
            restorePacmanPosition();

            // Všetkým coinom naspäť nastaví viditeľnosť na true
            foreach (Control c in this.Controls)
            {
                if (c is PictureBox && (string)c.Tag == "coin" )
                {
                    c.Visible = true;
                }
            }
        }


        void resetTimers()
        {
            gameTimer.Start();
            delayCounter.Start();
            ghostPathTimer.Start();
            ghostCalculateNewPath.Start();

            if (!optnsForm.gameDifficulty) // Ak je hard difficulty, tak spustíme aj pink ghosta
            {
                pinkGhostT.Start();
            }
            else
            {
                pinkGhost.Visible = false;
            }
        }
        
    }

    public static class PacmanMaps
    {
        /// <summary>
        /// 
        /// Uchováva informácie o mape -- počet riadkov/stĺpcov, veľkosť bunky, veľkosť celej obrazovky
        /// Layout mapy je reprezentovaný 0 a 1 -- 0 znamená stena, 1 znamená chodba.
        /// Na základe layoutu mapy inicializuje grid, podľa ktorého sa potom detekujú steny.
        /// 
        /// </summary>
        
        private const int numRows = 10;
        public const int numCols = 10;
        public const int windowSize = 360;
        public const int cellSize = 36;

        private static int[] pacmanStartingCoords = { 80, 300 };
        private static int[] blueGhostStartingCoords = { 144, 144 };
        private static int[] pinkGhostStartingCoords = { 252, 288 };

        static List<int[]> walls = new List<int[]>();

        public const string map = 
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

        public static void InitializeGrid()
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

        public static List<int[]> getWalls()
        {
            return walls;
        }

        public static int[] getStartingCoords(char entity)
        {
            if (entity == 'p')
            {
                return pacmanStartingCoords;
            }
            else if (entity == 'b')
            {
                return blueGhostStartingCoords;
            }
            else
            {
                return pinkGhostStartingCoords;
            }
            
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

        public const int pacmanSize = 22;
        public static int lives = 3;
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
                // abs. hodnota -- vzdialenosť môže byť sprava alebo zľava
                // wall[1] * PacmanMaps.cellSize: dáva reálnu súradnicu steny vo Forme
                // Od toho odčíta reálnu súradnicu pacmana + o koľko by sa posunul
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
            // Spočíta, koľko existuje pictureboxov s tagom "coin"
            
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
        /// <summary>
        /// 
        /// Trieda zodpovedá za ghosta a jeho pohyb. 
        /// Pamätá si pozíciu ghosta a pozíciu pacmana. Na základe toho vypočíta najkratšiu cestu k pacmanovi.
        /// Potom túto cestu zrekonštruuje a zapamätá si ju. Vytvorí frontu, do ktorej sa pridajú pohyby potrebné na dobehnutie pacmana.
        /// Na koniec má 3 metódy na správny alignment -- keď sa prepočíta nová cesta, tak ghost nemusí byť správne zarovnaný vo svojom políčku a preto
        /// ho tam treba posunúť.
        /// S triedou sú spojené 2 timery, jeden z nich každé 2s prepočíta najkratšiu cestu, druhý slúži na pohyb ghosta. Timery spravuje hlavná trieda.
        /// 
        /// </summary>

        System.Drawing.Color color;
        public int ghostSpeed = 6;

        private int[] pacmansPosition = new int[2]; // Aktuálna pozícia pacmana v rámci gridu
        public int[] ghostsPosition = new int[2];

        public List<int[]> currentShortestPath = new List<int[]>(); // Pamätá si súradnice jednotlivých políčok v rámci gridu
        public Queue<char> shortestPathQueue = new Queue<char>(); // Fronta s pohybmi, ktoré má ghost spraviť
        
        private int missalignmentX = 0;
        private int missalignmentY = 0;

        public Ghost(System.Drawing.Color color, int ghostX, int ghostY, int pacX, int pacY) 
        {
            this.color = color;
            findGhostsPosition(ghostX, ghostY);
            findPacmansPosition(pacX, pacY);

            if (optnsForm.currentAlgorithm == 0)
            {
                findShortestPathDijkstra();
            }
            else
            {
                findShortestPathBFS();
            }

            prepareQueue();
        }

        public string findPacmansPosition(int x, int y)
        {
            // Nájde aktuálnu pozíciu pacmana 
            int offset = Pacman.pacmanSize / 2;
            x = (x + offset) / PacmanMaps.cellSize;
            y = (y + offset) / PacmanMaps.cellSize;

            pacmansPosition = new int[2] { x, y };
            return x.ToString() + "," + y.ToString();
        }

        public string findGhostsPosition(int x, int y)
        {
            int offset = Pacman.pacmanSize / 2;
            x = (x + offset) / PacmanMaps.cellSize;
            y = (y + offset) / PacmanMaps.cellSize;

            ghostsPosition = new int[2] { x, y };
            return x.ToString() + "," + y.ToString();
        }

        public string findShortestPathBFS()
        {
            // Nájde najkratšiu cestu od ghosta k pacmanovi pomocou BFS.
            // Na začiatku vytvorí grid (podobne ako pri šachovnici). Steny na gride označí pomocou -2. Potom vytvorí frontu a spustí samotné BFS.
            // Po prebehnutí BFS zrekonštruuje najkratšiu cestu (zapamätá si konkrétne políčka na gride)

            // Vytvorenie gridu
            int[,] grid = new int[PacmanMaps.numCols, PacmanMaps.numCols];
            for (int i = 0; i < PacmanMaps.numCols; i++)
            {
                for (int j = 0; j < PacmanMaps.numCols; j++)
                {
                    grid[i, j] = -1;
                }
            }

            foreach (int[] wall in PacmanMaps.getWalls())
            {
                grid[wall[1], wall[0]] = -2;
            }

            // Začiatok BFS

            // Vytvorí prázdnu frontu, vloží do nej prvú pozíciu
            Queue<int[]> q = new Queue<int[]>(); 
            int[] startingCoords = { ghostsPosition[0], ghostsPosition[1], 0 };
            q.Enqueue(startingCoords);

            int[,] possibleMoves = { { 0, 1 }, { 1, 0 }, { 0, -1 }, { -1, 0 } };

            // Parent slúži na neskoršie zrekonštruovanie cesty
            int[,] parent = new int[10, 10]; 
            parent[ghostsPosition[0], ghostsPosition[1]] = -1;

            int result = -1; // result vydáva dĺžku cesty (potrebné aby sme vedeli, koľko krokov má cesta)

            while (q.Count > 0) // Kým fronta nie je prázdna (tj. kým sme nenavštívili všetky políčka)
            {
                // Vyberie prvok na začiatku fronty
                int[] current = q.Dequeue();
                int curx = current[0];
                int cury = current[1];
                int curLen = current[2];

                if (curx == pacmansPosition[0] && cury == pacmansPosition[1]) // Z cyklu vyskočí, ak sa pozície pacmana a ghosta zhodujú
                {
                    result = grid[curx, cury];
                    break;
                }

                for (int i = 0; i < possibleMoves.Length / 2; i++) // Skontroluje všetky možné pohyby z danej súradnice
                {
                    int possibleX = current[0] + possibleMoves[i, 0];
                    int possibleY = current[1] + possibleMoves[i, 1];

                    // Vyskúša jednotlivé možné pohyby, drží sa v rámci gridu a nevchádza do stien
                    if (possibleX >= 0 && possibleX < 10 && possibleY >= 0 && possibleY < 10 && grid[possibleX, possibleY] == -1 &&
                        grid[possibleX, possibleY] != -2)
                    {
                        int[] newCell = { possibleX, possibleY, curLen + 1 }; // Do fronty pridá vhodný pohyb
                        q.Enqueue(newCell);

                        parent[possibleX, possibleY] = curx * 10 + cury; // K rodičovi pridá dieťa -- najsledujúce políčko

                        grid[possibleX, possibleY] = curLen + 1; // Do políčka zapíše dĺžku najkratšej cesty
                    }
                }
            }

            if (result == -1) // Kebyže sa k pacmanovi nedá dostať
            {
                return result.ToString();
            }

            // Rekonštrukcia najkratšej cesty
            currentShortestPath = new List<int[]>();

            int x = pacmansPosition[0], y = pacmansPosition[1];

            for (int i = 0; i < result; i++)
            {
                int[] coords = { x, y };
                currentShortestPath.Insert(0, coords);
                int p = parent[x, y];
                x = p / 10;
                y = p % 10;
            }

            return result.ToString(); // Iba na printovanie dĺžky pri testovaní
        }

        public void findShortestPathDijkstra()
        {
            Tuple<int, int> start = new Tuple<int, int>(ghostsPosition[0], ghostsPosition[1]); // ghost coords
            Tuple<int, int> end = new Tuple<int, int>(pacmansPosition[0], pacmansPosition[1]); // pacman coords

            const int gridSize = 10;
            Tuple<int, int>[] possibleMoves = new Tuple<int, int>[] // možné pohyby po gride
                {
                    new Tuple<int, int>(0, -1), // left
                    new Tuple<int, int>(0, 1),   // right
                    new Tuple<int, int>(-1, 0), // up
                    new Tuple<int, int>(1, 0)  // down
                };

            HashSet<Tuple<int, int>> walls = new HashSet<Tuple<int, int>>(); // Tuple stien
            foreach (var wall in PacmanMaps.getWalls())
            {
                Tuple<int, int> tuple = new Tuple<int, int>(wall[1], wall[0]);
                walls.Add(tuple);
            }

            Dictionary<Tuple<int, int>, int> distances = new Dictionary<Tuple<int, int>, int>(); // Vzdialenosť od pozície ghosta k políčkam na gride   
            HashSet<Tuple<int, int>> unvisited = new HashSet<Tuple<int, int>>(); // Aby sme nechodili do tých istých políčok

            Dictionary<Tuple<int, int>, Tuple<int, int>> previous = new Dictionary<Tuple<int, int>, Tuple<int, int>>(); // Predchodca na rekonštrukciu cesty


            // Inicializácia algoritmu -- nastaví vzdialenosť na "nekonečno", predchodcov na žiadnych a všetky políčka na unvisited
            foreach (int row in Enumerable.Range(0, gridSize)) 
            {
                foreach (int col in Enumerable.Range(0, gridSize))
                {
                    Tuple<int, int> cell = new Tuple<int, int>(row, col);
                    distances[cell] = int.MaxValue;
                    previous[cell] = null;
                    unvisited.Add(cell);
                }
            }

            distances[start] = 0; // Vzdialenosť ghosta od samého seba je 0

            while (unvisited.Count > 0) // Algoritmus beží, kým sme nenavštívili všetky políčka
            {
                var current = unvisited.OrderBy(cell => distances[cell]).First(); // Vyberie políčko s najmenšou vzdialenosťou
                unvisited.Remove(current); // Označí ho za otvorené

                if (current.Equals(end)) // Koniec ak sa našla najkratšia cesta
                {
                    break;
                }

                foreach (var neighborOffset in possibleMoves)
                {
                    var neighborRow = current.Item1 + neighborOffset.Item1; // Vypočíta koordinácie suseda v gride
                    var neighborCol = current.Item2 + neighborOffset.Item2;
                    var neighbor = new Tuple<int, int>(neighborRow, neighborCol);

                    // Skontroluje, či sused je v rámci gridu a nie je v stene
                    if (neighborRow >= 0 && neighborRow < gridSize && neighborCol >= 0 && neighborCol < gridSize && !walls.Contains(neighbor)) 
                    {
                        var altDistance = distances[current] + 1; // Assuming a cost of 1 to move to a neighbor cell

                        if (altDistance < distances[neighbor]) // Ak nová vzdialenosť je menšia než tá, ktorú si pamätá, tak relaxuje
                        {
                            distances[neighbor] = altDistance;
                            previous[neighbor] = current;
                        }
                    }
                }
            }

            // Rekonštrukcia cesty, začína od konca
            var path = new List<Tuple<int, int>>();
            var currentCell = end;

            while (currentCell != null)
            {
                path.Add(currentCell);
                currentCell = previous[currentCell];
            }

            path.Reverse();
            currentShortestPath = path.Select(tuple => new int[] { tuple.Item1, tuple.Item2 }).ToList(); // Zmena tuplu do listu
        }

        public void prepareQueue()
        {
            // Do fronty pridáva kroky, ktoré má ghost spraviť, aby sa dostal k pacmanovi

            shortestPathQueue = new Queue<char>();
            int lastX = ghostsPosition[0];
            int lastY = ghostsPosition[1];
            foreach (var coord in currentShortestPath)
            {
                char command = ' ';
                if (coord[0] > lastX)
                {
                    command = 'r';
                }
                else if (coord[0] < lastX)
                {
                    command = 'l';
                }
                else if (coord[1] > lastY)
                {
                    command = 'u';
                }
                else if (coord[1] < lastY)
                {
                    command = 'd';
                }

                for (int i = 0; i < PacmanMaps.cellSize/ghostSpeed; i++)
                {
                    shortestPathQueue.Enqueue(command);
                }

                lastX = coord[0];
                lastY = coord[1];
            }
        }

        public char dequeue()
        {
            if (shortestPathQueue.Count == 0)
            {
                return 'e';
            }
            return shortestPathQueue.Dequeue();
        }

        public void calculateMissalignment(int x, int y)
        {
            // Slúži na zarovnanie ghosta v rámci jednej bunky gridu
            // Počíta, aká je jeho vzdialenosť od ľavého horného rohu bunky

            missalignmentX = x - (ghostsPosition[0] * PacmanMaps.cellSize);
            missalignmentY = y - (ghostsPosition[1] * PacmanMaps.cellSize);
        }

        public int[] align(char newDirection)
        {
            int[] temp;
            if (missalignmentX >= ghostSpeed && missalignmentY >= ghostSpeed)
            {
                temp = new int[2] { ghostSpeed, ghostSpeed};
                missalignmentX -= ghostSpeed;
                missalignmentY -= ghostSpeed;
            }
            else if (missalignmentX >= ghostSpeed && missalignmentY < ghostSpeed)
            {
                temp = new int[2] { ghostSpeed, missalignmentY };
                missalignmentX -= ghostSpeed;
                missalignmentY = 0;
            }
            else if (missalignmentX < ghostSpeed && missalignmentY >= ghostSpeed)
            {
                temp = new int[2] { missalignmentX, ghostSpeed };
                missalignmentX = 0;
                missalignmentY -= ghostSpeed;
            }
            else
            {
                temp = new int[2] { missalignmentX, missalignmentY };
                missalignmentX = 0;
                missalignmentY = 0;
            }

            return temp;
        }

        public bool isAlinged()
        {
            // Je ghost na správnej pozícii v rámci bunky?

            return (missalignmentX == 0 && missalignmentY == 0);
        }

        public void urobVsetkoCoSaDa(int ghostX, int ghostY, int pacmanX, int pacmanY)
        {
            findGhostsPosition(ghostX, ghostY);
            calculateMissalignment(ghostX, ghostY);
            findPacmansPosition(pacmanX, pacmanY);

            if (optnsForm.currentAlgorithm == 0)
            {
                findShortestPathDijkstra();
            }
            else
            {
                findShortestPathBFS();
            }
            
            prepareQueue();
        }
    }
}
