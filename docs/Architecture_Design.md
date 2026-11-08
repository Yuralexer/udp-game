# Архитектура проекта

## Жанр и топология
Жанр: FPS. 
Топология: **Dedicated server**.

## Распределение обязанностей
| Компонент | Обязанности |
|---|---|
| Сервер | Принимает команды, проверяет их, хранит состояние мира, рассылает обновления, ведёт лог |
| Клиент | Считывает ввод, отправляет команды, отображает состояние, полученное от сервера |

## Диаграмма классов
```mermaid
classDiagram
    class ServerManager {
        +Run()
        +HandlePacket(packet, endpoint)
    }
    class World {
        +Dictionary~int, Player~ Players
        +ApplyMovement(playerId, x, y, z)
        +ApplyShoot(playerId, weaponId)
    }
    class Player {
        +int Id
        +string Login
        +Vector3 Position
    }
    class ClientNetwork {
        +Connect()
        +SendCommand(type, payload)
    }
    class ClientRenderer {
        +Render(worldState)
    }
    class PacketCodec {
        +Encode()
        +TryDecode()
    }
    ServerManager --> World
    World --> Player
    ServerManager --> PacketCodec
    ClientNetwork --> PacketCodec
    ClientNetwork --> ClientRenderer
```

## Поток данных
Клиент → (MOVEMENT/SHOOT по UDP) → Сервер → обновляет World → (POSITION_UPDATE) → Клиент → Renderer