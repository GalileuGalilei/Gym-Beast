using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    private Collider col;
    [SerializeField] float highlightPower = 1.5f;
    public List<BotController> botInRange = new();

    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<Collider>();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bot"))
        {
            botInRange.Add(other.gameObject.GetComponent<BotController>());
            //highlight the bot
            Renderer r = other.gameObject.GetComponentInChildren<Renderer>();
            r.material.color *= highlightPower;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Bot"))
        {
            botInRange.Remove(other.gameObject.GetComponent<BotController>());
            //unhighlight the bot
            Renderer r = other.gameObject.GetComponentInChildren<Renderer>();
            r.material.color /= highlightPower;
        }
    }
}
