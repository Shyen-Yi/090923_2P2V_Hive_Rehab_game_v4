using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

public class bgWormholeManager : MonoBehaviour
{
    public SpriteRenderer bgWormhole;
    public Sprite bgState01;
    public Sprite bgState02;
    public Sprite bgState03;
    public Sprite bgState04;
    public Sprite bgState05;
    public Sprite bgState06;
    public Sprite bgState07;
    public Sprite bgState08;
    public Sprite bgState09;
    public Sprite bgState10;

    public Sprite[] bgStates;

    public float animationSpeed = 0.5f;
    public int currentIndex = 0;

    void Start() 
    {
        bgStates = new Sprite[] 
        {
            bgState01,
            bgState02,
            bgState03,
            bgState04,
            bgState05,
            bgState06,
            bgState07,
            bgState08,
            bgState09,
            bgState10,
        };
        Debug.Log(bgStates[0]);

        StartCoroutine(loopAnimation());
    }

    IEnumerator loopAnimation()
    {
        while(true)
        {
            Sprite currentSprite = bgStates[currentIndex];
            bgWormhole.sprite = currentSprite;
            currentIndex = (currentIndex + 1) % bgStates.Length;
            yield return new WaitForSeconds(animationSpeed);
        }   
    }

}
