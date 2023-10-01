using UnityEngine;
using UnityEngine.UI;

public class HandMovement : MonoBehaviour
{
    public GameManagerScriptableObject gameManager;
    private void Update()
    {
        if (gameManager.isGameRunning == true){
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Cursor.visible = false;
            mousePosition.z = 0f;
            this.transform.position = mousePosition;
        }
        else
        {
            Cursor.visible = true;
        };
    }
}

