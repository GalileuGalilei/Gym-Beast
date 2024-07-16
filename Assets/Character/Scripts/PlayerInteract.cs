using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] Material highlightMaterial;
    [SerializeField] Material originalMaterial;
    public List<BotController> botInRange = new();

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bot"))
        {
            BotController bot = other.GetComponent<BotController>();
            botInRange.Add(bot.GetComponent<BotController>());
            //highlight the bot
            Renderer r = other.gameObject.GetComponentInChildren<Renderer>();
            r.material = highlightMaterial;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Bot"))
        {
            BotController bot = other.GetComponent<BotController>();

            if (!botInRange.Contains(bot))
            {
                return;
            }

            botInRange.Remove(bot);
            //unhighlight the bot
            Renderer r = other.gameObject.GetComponentInChildren<Renderer>();
            r.material = originalMaterial;
        }
    }
}
