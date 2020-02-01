using UnityEngine;

public class BrokenShip : MonoBehaviour
{
    public GameSettings.Team Team { get; set; }

    [SerializeField] private int _numPiecesToComplete;

    public void AddPart(ShipPart part)
    {
        part.gameObject.SetActive(false);
        --_numPiecesToComplete;

        if(_numPiecesToComplete <= 0)
        {
            Debug.Log($"{Team.ToString()} team wins!");
        }
    }
}
