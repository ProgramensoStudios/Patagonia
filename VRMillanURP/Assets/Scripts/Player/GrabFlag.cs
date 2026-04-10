using UnityEngine;

public class GrabFlag : MonoBehaviour
{
    [SerializeField] private GameObject book;
    [SerializeField] private GameObject ticket;
    [SerializeField] private ParticleSystem fuegos;
   public void Grab()
   {
        book.SetActive(true);
        ticket.SetActive(true);
        Debug.Log("Ganaste agarra el premio");
        fuegos.Play();
   }
}
