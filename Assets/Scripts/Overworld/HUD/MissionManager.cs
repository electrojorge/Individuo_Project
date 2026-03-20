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
        hud.SetMainMission("INVESTIGA LA MANSIÓN");

        hud.ClearSubMission();

        hud.SetSubMission(0, "BUSCA PISTAS EN LA MANSIÓN SOBRE EL ASESINATO");
    }

    public void EnemyAppears()
    {
        hud.SetMainMission("¡UN ENEMIGO APARECE!");

        hud.ClearSubMission();

        hud.SetSubMission(0, "ACABA CON EL ENEMIGO");
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
