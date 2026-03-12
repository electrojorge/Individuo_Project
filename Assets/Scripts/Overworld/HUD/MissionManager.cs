using UnityEngine;

public class MissionManager : MonoBehaviour
{
    public static MissionManager instance;

    public MissionHUD hud;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        StartInvestigationMission();
    }

    void StartInvestigationMission()
    {
        hud.SetMainMission("Investiga el asesinato");

        hud.ClearSubMission();

        hud.SetSubMission(0, "Busca pistas en la mansión");
    }

    public void EnemyAppears()
    {
        hud.SetMainMission("¡Un enemigo aparece!");

        hud.ClearSubMission();

        hud.SetSubMission(0, "Acaba con el enemigo");
    }

    public void EnemyDefeated()
    {
        hud.SetMainMission("Continúa investigando");

        hud.ClearSubMission();

        hud.SetSubMission(0, "Recoge la llave");
    }

    public void KeyCollected()
    {
        hud.SetMainMission("Abre la puerta cerrada");

        hud.ClearSubMission();

        hud.SetSubMission(0, "Usa la llave en la puerta");
    }
}
