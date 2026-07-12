namespace Katana.Combat
{
    public interface ICombatant
    {
        void RequestAttack();
        void RequestAbility(int slotIndex);
    }
}
