using Nerdbank.MessagePack;

namespace NorthSouthSystems.Entities;

public readonly record struct EntityChange(
    [property: Key(1)] string Name,
    [property: Key(2)] int Added,
    [property: Key(3)] int Modified,
    [property: Key(4)] int Deleted);