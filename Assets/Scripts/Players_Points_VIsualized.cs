using TMPro;
using UnityEngine;

public class Players_Points_VIsualized : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI playersPointLbl;

    public void SetPlayerPoints(Player player)
    {
        playersPointLbl.text = player.GetPoints().ToString();
    }
}
