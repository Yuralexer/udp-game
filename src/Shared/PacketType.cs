namespace Shared;

public enum PacketType : byte
{
    Auth           = 1,   // client → server: login & password
    AuthResult     = 2,   // server → client: 1 = success, 0 = fail
    Movement       = 3,   // client → server: new coordinates
    PositionUpdate = 4,   // server → client: approved position
    Shoot          = 5,   // client → server: shot
    ShootResult    = 6,   // server → client: shot result
    Error          = 255  // server → client: error
}