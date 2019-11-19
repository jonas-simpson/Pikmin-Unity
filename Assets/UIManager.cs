using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text withPlayer, fieldCount;
    // Start is called before the first frame update
    void Start()
    {
        withPlayer = GameObject.Find("Pikmin with Player").GetComponent<Text>();
        fieldCount = GameObject.Find("Pikmin in Field").GetComponent<Text>();
    }

    public void UpdateFieldCount(int count)
    {
        fieldCount.text = count.ToString();
    }

    public void ChangeCount(int amt, string count)
    {
        if(count == "withPlayer")
        {
            withPlayer.text = amt.ToString();
        }
        else if(count == "field")
        {
            fieldCount.text = amt.ToString();
        }
    }
}
