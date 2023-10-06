using UnityEngine;

public class FigureClean : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        AudioService audioService = ServiceLocator.Current.Get<AudioService>();
        GameState gameState = ServiceLocator.Current.Get<GameState>();
        Destroy(collision.gameObject);
        if (gameState.State == State.TETRIS 
            || gameState.State == State.WALK)
        {
            if(collision.GetComponent<Figure>().Index != 18)
                audioService.PlayBreakDish();
            else
                audioService.PlaySpoilerFall();
        }
    }
}
