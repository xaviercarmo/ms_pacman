using UnityEngine;
using System.Collections;

public class MainMenuButtonHandler : MonoBehaviour
{
    public GameObject ButtonSelector;
    public AudioSource SelectNoise;

    Vector3 ButtonSelectorPosition;

    void Awake()
    {
        var rectTransform = transform as RectTransform;
        ButtonSelectorPosition = new Vector3(rectTransform.position.x - rectTransform.rect.width / 2 - 30, ButtonSelector.transform.position.y);
    }

    void Start()
    {
    }

    void Update()
    {
    }

    public void PointerEnter()
    {
        if (Mathf.Abs(ButtonSelector.transform.position.x - ButtonSelectorPosition.x) > 1)
        {
            SelectNoise.Play();
            ButtonSelector.transform.position = ButtonSelectorPosition;
        }
    }

    public void LoadOriginalLevel()
    {
        GameManager.SetLevel(GameLevel.OriginalLevel);
    }

    public void LoadNewLevel()
    {
        GameManager.SetLevel(GameLevel.NewLevel);
    }
}
