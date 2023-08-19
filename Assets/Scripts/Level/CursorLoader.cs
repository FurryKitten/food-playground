using UnityEngine;

public class CursorLoader : MonoBehaviour
{
    [SerializeField] private Texture2D _cursorTexture;

    private void Start()
    {
        Cursor.SetCursor(_cursorTexture, Vector2.zero, CursorMode.Auto);
    }
}
