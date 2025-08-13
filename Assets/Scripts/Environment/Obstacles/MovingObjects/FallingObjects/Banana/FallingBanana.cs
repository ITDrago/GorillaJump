using Core.Game;
using Player;

namespace Environment.Obstacles.MovingObjects.FallingObjects.Banana
{
    public class FallingBanana : MovingObject
    {
        protected override void OnPlayerEnter(PlayerCore playerCore)
        {
            GameEvents.BananaCollected();
            
            if (playerCore.PlayerShieldController) playerCore.PlayerShieldController.ActivateShield();
                
            Destroy(gameObject);
        }   
    }
}