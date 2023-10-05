using UnityEngine;

public class FigureClean : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(collision.gameObject);
        if (ServiceLocator.Current.Get<GameState>().State == State.TETRIS 
            || ServiceLocator.Current.Get<GameState>().State == State.WALK)
        {
            if(collision.GetComponent<Figure>().Index != 18)
                ServiceLocator.Current.Get<AudioService>().PlayBreakDish();
        }
    }
}
