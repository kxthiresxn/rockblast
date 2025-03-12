public static class Constants
    {
        public const float MAX_FIRE_COOLDOWN = 0.065f;

        public const float MAX_X_VELOCITY = 4f;

        public const int TOP_BORDER = 5;
        
        public const int BULLET_DAMAGE = 1;

        public const int MAX_BALLS_ON_SCREEN = 2;

        public const int SPLIT_BALL_INIT_X_VELOCITY = 4;
        
        public const int COIN_VALUE = 1;
    }

    public enum Direction : int
    {
        None = 0,
        Left = 1,
        Right = 2,
        Top = 3,
        Bottom = 4
    }

    public enum BallSize : int
    {
        Level_0 = 0,
        Level_1 = 1,
        Level_2 = 2,
        Level_3 = 3
    }

    public enum GameState : int
    {
        MainMenu,
        InGame,
        Paused,
        GameOver,
        BeginGame
    }
