using UnityEngine;
using UnityEngine.Tilemaps;

public class FishingPoint : MonoBehaviour
{
    [SerializeField] private PlayerController _playerController;
    private void Update()
    {
        if(_playerController.waterTilemap.HasTile(_playerController.waterTilemap.WorldToCell(transform.position)))
            _playerController.CanFish = true;
        else
            _playerController.CanFish = false;
    }
}
