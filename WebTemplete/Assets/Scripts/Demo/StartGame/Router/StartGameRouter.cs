using UnityEngine;

public class StartGameRouter : IRouter
{
    public void Init()
    {
        UiController.Instance.GetWindow<StartGameWindow>().Clicked += OpenGameScene;
    }

    private void OpenGameScene()
    {
        SceneController.Instance.LoadScene(SceneList.Game);
    }

    public void Exit()
    {
        UiController.Instance.GetWindow<StartGameWindow>().Clicked -= OpenGameScene;
    }
}