using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuConfig : MonoBehaviour {
    public GameObject slider = null;
    
    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void StartGame() {
        Debug.Log(SceneManager.GetActiveScene().buildIndex);
        Debug.Assert (slider != null);
        Game.GameController.maxRounds = GetSliderValue();
        SceneManager.LoadScene("GameStage");
        Game.GameController.StartGameStage();
    }

    private int GetSliderValue() {
        return Mathf.FloorToInt(slider.GetComponent<Slider>().value);
    }
}
