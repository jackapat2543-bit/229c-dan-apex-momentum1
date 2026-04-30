using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
  public void start_game ()
  {
    SceneManager.LoadScene(1);
  }
  public void main_menu()
  {
     SceneManager.LoadScene(0);
  }
  public void load_lose_scene()
{
    SceneManager.LoadScene(2);
}

}