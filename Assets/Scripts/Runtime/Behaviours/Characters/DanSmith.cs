namespace Universal.Runtime.Behaviours.Characters
{
    public class DanSmith : Character
    {
        public override string CharacterName => "Dan Smith";

        public override void Activate()
        {
            base.Activate();
            // Dan-specific activation logic
        }

        public override void Deactivate()
        {
            base.Deactivate();
            // Garcian-specific deactivation logic
        }
    }
}