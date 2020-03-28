using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TypeManager : MonoBehaviour
{

    [SerializeField] public GameObject TypeItem;
    [SerializeField] RectTransform TypeItemArea;
    GameManager gameManager;
    // [SerializeField] public string[] TypeName;



    // Start is called before the first frame update
    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    void Start()
    {
        for (int i = 0; i < gameManager.QuestionTypeLevels.Length; i++)
        {
            GenTypes(i);
        }
    }


    void GenTypes(int id)
    {

        GameObject newTypePrefab = (GameObject)Instantiate(TypeItem, TypeItemArea);
        newTypePrefab.GetComponentInChildren<Text>().text = gameManager.QuestionTypeLevels[id].QuestionType;


    }

    // Update is called once per frame
    void Update()
    {

    }
}
