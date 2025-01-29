using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SequenceIndicator : MonoBehaviour
{
    [SerializeField]
    private Image image;

    private Color baseColor, wrongColor;

    private bool isWrong;
    private float flashTimerMax, flashTimerCurrent;

    // Start is called before the first frame update
    void Start()
    {
        baseColor = new Color(0.0f, 1.0f, 0.0f);
        wrongColor = new Color(1.0f, 0.0f, 0.0f);

        isWrong = false;
        flashTimerMax = 0.5f;
        flashTimerCurrent = flashTimerMax;
    }

    private void FixedUpdate() {
        if(GameManager.instance.CurrentGameState == GameState.Game
            && isWrong) {
            flashTimerCurrent -= Time.deltaTime;
            if(flashTimerCurrent <= 0.0f) {
                ResetIndicatorImage();
            }
        }
    }

    public void FlashIndicatorImage() {
        isWrong = true;
        flashTimerCurrent = flashTimerMax;
        image.color = wrongColor;
    }

    private void ResetIndicatorImage() {
        isWrong = false;
        image.color = baseColor;
    }
}
