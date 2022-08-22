public class Constants
{
    public const int MAX_PLAYERS_PER_MATCH = 10;
    public const float MAX_DIST_FROM_CURSOR_TO_BALL = 7f;
    public const float MIN_DIST_FROM_CURSOR_TO_BALL = 1f;
    public const int SEND_PLAYER_POSITION_FREQUENCY = 40;
}

public class NetworkMessageType
{
    public const byte REQUEST_MATCH_DATA = 0;
    public const byte BALL_POSITION_UPDATE = 1;
    public const byte PLAYER_POSITION_UPDATE = 2;
}


