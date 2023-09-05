using System;

public class Ghost
{
    public class Ghost
    {
        /// <summary>
        /// 
        /// Trieda zodpovedá za ghosta a jeho pohyb. Pamätá si pozíciu ghosta a pozíciu pacmana. Na základe toho vypočíta najkratšiu cestu k pacmanovi.
        /// Potom túto cestu zrekonštruuje a zapamätá si. Násladne sa vytvorí fronta, do ktorej sa pridajú pohyby potrebné na dobehnutie pacmana.
        /// Na koniec má 3 metódy na správny alignment -- keď sa prepočíta nová cesta, tak ghost nemusí byť správne zarovnaný vo svojom políčku a preto
        /// ho tam treba posunúť.
        /// S triedou sú spojené 2 timery, jeden z nich každé 2s prepočíta najkratšiu cestu, druhý slúži na pohyb ghosta.
        /// 
        /// </summary>

        System.Drawing.Color color;
        private int ghostSpeed = 6;

        private int[] pacmansPosition = new int[2]; // Aktuálna pozícia pacmana v rámci gridu
        public int[] ghostsPosition = new int[2];

        public List<int[]> currentShortestPath = new List<int[]>(); // Pamätá si súradnice jednotlivých políčok v rámci gridu
        public Queue<char> shortestPathQueue = new Queue<char>(); // Fronta s pohybmi, ktoré má ghost spraviť

        private int missalignmentX = 0;
        private int missalignmentY = 0;

        public Ghost(System.Drawing.Color color)
        {
            this.color = color;
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

        public string findShortestPath()
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

            Queue<int[]> q = new Queue<int[]>();
            int[] startingCoords = { ghostsPosition[0], ghostsPosition[1], 0 };
            q.Enqueue(startingCoords);

            int[,] possibleMoves = { { 0, 1 }, { 1, 0 }, { 0, -1 }, { -1, 0 } };

            int[,] parent = new int[10, 10]; // Parent slúži na neskoršie zrekonštruovanie cesty
            parent[ghostsPosition[0], ghostsPosition[1]] = -1;

            int result = -1;
            while (q.Count > 0)
            {
                int[] current = q.Dequeue();
                int curx = current[0];
                int cury = current[1];
                int curLen = current[2]; // Na nájdenie dĺžky najkratšej cesty (potrebné aby sme vedeli, koľko krokov má cesta)

                if (curx == pacmansPosition[0] && cury == pacmansPosition[1]) // Z cyklu vyskočí, ak sa pozície pacmana a ghosta zhodujú
                {
                    result = grid[curx, cury];
                    break;
                }

                for (int i = 0; i < possibleMoves.Length / 2; i++)
                {
                    int possibleX = current[0] + possibleMoves[i, 0];
                    int possibleY = current[1] + possibleMoves[i, 1];

                    // Vyskúša jednotlivé možné pohyby, drží sa v rámci gridu a nevchádza do stien
                    if (possibleX >= 0 && possibleX < 10 && possibleY >= 0 && possibleY < 10 && grid[possibleX, possibleY] == -1 &&
                        grid[possibleX, possibleY] != -2)
                    {
                        int[] newVer = { possibleX, possibleY, curLen + 1 }; // Do fronty pridá vhodný pohyb
                        q.Enqueue(newVer);

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

            return result.ToString();
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

                for (int i = 0; i < PacmanMaps.cellSize / ghostSpeed; i++)
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

        public int[] align()
        {
            int[] temp;
            if (missalignmentX >= ghostSpeed && missalignmentY >= ghostSpeed)
            {
                temp = new int[2] { ghostSpeed, ghostSpeed };
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

    }
}
