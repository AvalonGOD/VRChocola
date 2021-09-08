using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpellSpawner : MonoBehaviour
{

    
    public List<GameObject> objects;
    private GameObject itemPref;
    

    private void Start()
    {
        
    }

    public void SpellSpawn(string objectName)
    {
        foreach (var item in objects)
        {
            item.SetActive(objectName == item.name);
            // itemPref = item;
            StartCoroutine(corutine(item));
            //Destroy(Instantiate(item, spawnPoint.transform.position, spawnPoint.transform.rotation), 3f);
            
        }
    }

    

    IEnumerator corutine(GameObject item)
    {
        yield return new WaitForSeconds(2);
        
        Destroy(Instantiate(item,transform.position, transform.rotation), 3f);
    }

}
