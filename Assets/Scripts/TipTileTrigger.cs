using UnityEngine;
using UnityEngine.Tilemaps;

public class TileTipTrigger : MonoBehaviour
{
    public Tilemap tipTilemap;
    public TipPopupUI tipUI;

    void Update()
    {
        Vector3 playerPos = transform.position;
        Vector3Int cellPos = tipTilemap.WorldToCell(playerPos);

        TileBase tile = tipTilemap.GetTile(cellPos);
        if (tile is TipTile tipTile)
        {
            tipUI.ShowTip(tipTile.tipText);
        }
        else
        {
            tipUI.HideTip(); // Optional: auto-hide when stepping off
        }
    }
}