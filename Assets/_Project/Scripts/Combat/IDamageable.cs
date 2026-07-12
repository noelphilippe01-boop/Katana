using Katana.Core;

namespace Katana.Combat
{
    public interface IDamageable
    {
        bool IsAlive { get; }
        void ApplyDamage(DamageInfo damage);
    }
}
