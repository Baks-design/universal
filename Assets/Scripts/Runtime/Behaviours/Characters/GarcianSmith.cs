namespace Universal.Runtime.Behaviours.Characters
{
    public class GarcianSmith : Character
    {
        public override string CharacterName => "Garcian Smith";

        public override void Activate()
        {
            base.Activate();
            // Garcian-specific activation logic
        }

        public override void Deactivate()
        {
            base.Deactivate();
            // Garcian-specific deactivation logic
        }
    }
}