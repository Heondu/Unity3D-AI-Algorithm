using UnityEngine;
using AIAlgorithm.AStar;

public class Agent : MonoBehaviour
{
    [SerializeField] private GameObject selectUI;

    public bool IsSelected = false;

    private AStar aStar;

    private void Start()
    {
        aStar = gameObject.GetComponent<AStar>();
    }

    public void ToggleSelectUI()
    {
        selectUI.SetActive(IsSelected);
    }

    public void MoveTo(Vector3 destination)
    {
        aStar.StartFinding(destination);
    }
}
