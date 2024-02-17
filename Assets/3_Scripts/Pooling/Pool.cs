using UnityEngine;

public class Pool
{
    static Pool m_instance;
    public static Pool Instance
    {
        get {
            if (m_instance == null)
                m_instance = new Pool();
            return m_instance;
        }
    }

    Pool()
    {
        ParticleSystems = new PooledObjects<ParticleSystem>();
        TowerTiles = new PooledObjects<TowerTile>();
    }

    public PooledObjects<ParticleSystem> ParticleSystems { get; }

    public PooledObjects<TowerTile> TowerTiles { get; }
}
