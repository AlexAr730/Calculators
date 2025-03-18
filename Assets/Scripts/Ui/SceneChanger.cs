using UnityEngine;
using UnityEngine.SceneManagement;

//Clase que cambia de escena
public class SceneChanger : MonoBehaviour
{
    public string sceneName; // Nombre de la escena a cargar

    public void ChangeScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
