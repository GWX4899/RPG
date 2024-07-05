using UnityEngine;


public interface IDamage
{
    void Hurted(float damage);
    void Hurted(string HitAnimation);
    void Hurted(float damage, string HitAnimation,Transform attacker);
}
