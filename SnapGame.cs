using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SnakeGame
{
    // Enum to define the possible directions for the snake
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    // Represents a single segment of the snake's body
    public class SnakeSegment
    {
        public int X { get; set; } // X-coordinate of the segment
        public int Y { get; set; } // Y-coordinate of the segment

        public SnakeSegment(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    // Represents the food item
    public class Food
    {
        public int X { get; set; } // X-coordinate of the food
        public int Y { get; set; } // Y-coordinate of the food

        public Food(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public class Program
    {
        // Game board dimensions
        private static int boardWidth = 60;
        private static int boardHeight = 20;

        // Game state variables
        private static List<SnakeSegment> snake;
        private static Food food;
        private static Direction currentDirection;
        private static bool gameOver;
        private static int score;
        private static int gameSpeed = 150; // Milliseconds delay per frame (lower is faster)
        private static Random random = new Random();

        // Main entry point of the game
        static void Main(string[] args)
        {
            // Initial setup of the console window
            Console.Title = "Basic C# Snake Game";
            Console.CursorVisible = false; // Hide the blinking cursor
            Console.SetWindowSize(boardWidth + 2, boardHeight + 3); // Set console window size
            Console.SetBufferSize(boardWidth + 2, boardHeight + 3); // Prevent scrollbars

            InitializeGame(); // Setup initial game state

            // Main game loop
            while (!gameOver)
            {
                HandleInput();   // Check for user input
                UpdateGame();    // Update snake position, check collisions
                DrawGame();      // Render the game state to the console
                Thread.Sleep(gameSpeed); // Pause for game speed
            }

            // Game over screen
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition(boardWidth / 2 - 5, boardHeight / 2 - 1);
            Console.WriteLine("GAME OVER!");
            Console.SetCursorPosition(boardWidth / 2 - 9, boardHeight / 2 + 1);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Final Score: {score}");
            Console.SetCursorPosition(boardWidth / 2 - 14, boardHeight / 2 + 3);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey(); // Wait for user input before closing
        }

        // Initializes all game variables and elements
        static void InitializeGame()
        {
            Console.Clear(); // Clear any previous console content
            snake = new List<SnakeSegment>();
            // Start snake in the middle of the board, 3 segments long
            snake.Add(new SnakeSegment(boardWidth / 2, boardHeight / 2));
            snake.Add(new SnakeSegment(boardWidth / 2 - 1, boardHeight / 2));
            snake.Add(new SnakeSegment(boardWidth / 2 - 2, boardHeight / 2));

            currentDirection = Direction.Right; // Initial movement direction
            gameOver = false;
            score = 0;

            GenerateFood(); // Place the first food item
            DrawBorder();   // Draw the game border
        }

        // Handles user input (arrow keys for movement, 'Q' to quit)
        static void HandleInput()
        {
            if (Console.KeyAvailable) // Check if a key has been pressed
            {
                ConsoleKeyInfo key = Console.ReadKey(true); // Read key without displaying it

                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                        // Prevent snake from reversing directly into itself
                        if (currentDirection != Direction.Down)
                            currentDirection = Direction.Up;
                        break;
                    case ConsoleKey.DownArrow:
                        if (currentDirection != Direction.Up)
                            currentDirection = Direction.Down;
                        break;
                    case ConsoleKey.LeftArrow:
                        if (currentDirection != Direction.Right)
                            currentDirection = Direction.Left;
                        break;
                    case ConsoleKey.RightArrow:
                        if (currentDirection != Direction.Left)
                            currentDirection = Direction.Right;
                        break;
                    case ConsoleKey.Q: // 'Q' to quit the game immediately
                        gameOver = true;
                        break;
                }
            }
        }

        // Updates the game state for each frame
        static void UpdateGame()
        {
            // Get the current head of the snake
            SnakeSegment head = snake[0];
            SnakeSegment newHead = new SnakeSegment(head.X, head.Y); // Create a new head for movement

            // Move the new head based on current direction
            switch (currentDirection)
            {
                case Direction.Up:
                    newHead.Y--;
                    break;
                case Direction.Down:
                    newHead.Y++;
                    break;
                case Direction.Left:
                    newHead.X--;
                    break;
                case Direction.Right:
                    newHead.X++;
                    break;
            }

            // Check for collisions
            CheckCollision(newHead);

            if (gameOver) return; // If collision detected, end update

            // Check if snake ate food
            if (newHead.X == food.X && newHead.Y == food.Y)
            {
                score += 10; // Increase score
                gameSpeed = Math.Max(50, gameSpeed - 5); // Increase speed (min 50ms)
                GenerateFood(); // Generate new food
            }
            else
            {
                // If no food eaten, remove the tail to simulate movement
                Console.SetCursorPosition(snake.Last().X + 1, snake.Last().Y + 1);
                Console.Write(" "); // Erase the old tail
                snake.RemoveAt(snake.Count - 1);
            }

            // Add the new head to the beginning of the snake list
            snake.Insert(0, newHead);
        }

        // Checks for wall and self-collisions
        static void CheckCollision(SnakeSegment head)
        {
            // Wall collision
            if (head.X < 0 || head.X >= boardWidth ||
                head.Y < 0 || head.Y >= boardHeight)
            {
                gameOver = true;
                return;
            }

            // Self-collision (check new head against all existing body segments)
            // Start from index 1 to avoid comparing head with itself
            for (int i = 1; i < snake.Count; i++)
            {
                if (head.X == snake[i].X && head.Y == snake[i].Y)
                {
                    gameOver = true;
                    return;
                }
            }
        }

        // Generates a new random position for the food
        static void GenerateFood()
        {
            // Loop until a position is found that's not on the snake
            while (true)
            {
                int foodX = random.Next(0, boardWidth);
                int foodY = random.Next(0, boardHeight);

                bool collisionWithSnake = false;
                foreach (var segment in snake)
                {
                    if (segment.X == foodX && segment.Y == foodY)
                    {
                        collisionWithSnake = true;
                        break;
                    }
                }

                if (!collisionWithSnake)
                {
                    food = new Food(foodX, foodY);
                    break;
                }
            }
        }

        // Draws all game elements to the console
        static void DrawGame()
        {
            // Draw food
            Console.ForegroundColor = ConsoleColor.Green;
            Console.SetCursorPosition(food.X + 1, food.Y + 1); // +1 because of border
            Console.Write("F");

            // Draw snake (head is different color)
            for (int i = 0; i < snake.Count; i++)
            {
                if (i == 0) // Head
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }
                else // Body
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                }
                Console.SetCursorPosition(snake[i].X + 1, snake[i].Y + 1);
                Console.Write("O");
            }

            // Update score display
            Console.SetCursorPosition(0, boardHeight + 1);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"Score: {score} | Speed: {1000 / gameSpeed} units/s ");
            Console.ResetColor(); // Reset color to default
        }

        // Draws the border of the game board
        static void DrawBorder()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            // Top border
            for (int i = 0; i < boardWidth + 2; i++)
            {
                Console.SetCursorPosition(i, 0);
                Console.Write("#");
            }
            // Bottom border
            for (int i = 0; i < boardWidth + 2; i++)
            {
                Console.SetCursorPosition(i, boardHeight + 1);
                Console.Write("#");
            }
            // Left border
            for (int i = 0; i < boardHeight + 2; i++)
            {
                Console.SetCursorPosition(0, i);
                Console.Write("#");
            }
            // Right border
            for (int i = 0; i < boardHeight + 2; i++)
            {
                Console.SetCursorPosition(boardWidth + 1, i);
                Console.Write("#");
            }
            Console.ResetColor();
        }
    }
}
