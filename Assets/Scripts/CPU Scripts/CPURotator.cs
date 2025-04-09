public class CPURotator : PlayerRotator
{
    protected override void OnEnable()
    {
        // Skip enabling player inputs
    }

    protected override void OnDisable()
    {
        // Skip disabling player inputs
    }

    public void RotateProgrammatically(int direction)
    {
        Rotate(direction);
    }

    public void UseAbilityProgrammatically(AbilityDirection direction)
    {
        UseAbility(direction);
    }
}
