namespace BattleShip.API;

public class Game : IDisposable
{
    public readonly Guid Id;

    public BaseController? Player1Controller { get; set; }
    public BaseController? Player2Controller { get; set; }
    
    private BaseController? _currentPlayer;
    
    private bool _isRunning;
    private readonly Thread _gameThread;

    public Game()
    {
        Id = Guid.NewGuid();
        _isRunning = true;
        _gameThread = new Thread(RunGame);
    }

    public void Dispose()
    {
        if (_currentPlayer is not null)
        {
            // TODO cancel current player's turn
        }
        _isRunning = false;
        _gameThread.Join();
    }

    private void RunGame()
    {
        while (_isRunning)
        {
            // handle player 1
            while (Player1Controller is null)
            {
                Thread.Sleep(100);
            }
            HandlePlayer(Player1Controller);
            // handle player 2
            while (Player2Controller is null)
            {
                Thread.Sleep(100);
            }
            HandlePlayer(Player2Controller);
        }
    }
    
    private void HandlePlayer(BaseController playerController)
    {
        // handle player
        _currentPlayer = playerController;
    }
    
}