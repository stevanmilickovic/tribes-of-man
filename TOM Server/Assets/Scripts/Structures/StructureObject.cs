
public class StructureObject
{

    public Structure structure;
    public int health;
    public bool destroyed;
    public int destroyedHealth;

    public StructureObject(Structure _structure, bool _destroyed, int _health, int _destroyedHealth)
    {
        structure = _structure;
        health = _health;
        destroyed = _destroyed;
        destroyedHealth = _destroyedHealth;
    }

    public StructureObject(Structure _structure)
    {
        structure = _structure;
        destroyed = false;
        health = _structure.maxHealth;
        destroyedHealth = _structure.maxBrokenHealth;
    }
}
