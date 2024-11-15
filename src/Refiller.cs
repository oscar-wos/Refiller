using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace Refiller;

public partial class Refiller : BasePlugin, IPluginConfig<RefillerConfig>
{
    public void OnConfigParsed(RefillerConfig config)
    {
        Config = config;
    }

    public override void Load(bool isReload)
    {
        RegisterEventHandler<EventPlayerDeath>(OnPlayerDeath);
    }

    public HookResult OnPlayerDeath(EventPlayerDeath @event, GameEventInfo info)
    {
        var victim = @event.Userid;

        if (victim == null || !victim.IsValid)
            return HookResult.Continue;

        List<CCSPlayerController?> players =
        [
            @event.Attacker,
            Config.AssistRefill ? @event.Assister : null
        ];

        List<CHandle<CBasePlayerWeapon>> weapons = [];

        foreach (var player in players.Where(player => player != null && player.IsValid))
        {
            if (Config.AmmoRefill == "all")
                weapons.AddRange(player!.PlayerPawn!.Value!.WeaponServices!.MyWeapons);
            else if (Config.AmmoRefill == "current")
                weapons.Add(player!.PlayerPawn!.Value!.WeaponServices!.ActiveWeapon);
        }

        Server.NextFrame(() =>
        {
            foreach (var weapon in weapons.Where(w => w != null && w.IsValid))
            {
                var weaponData = weapon.Value?.As<CCSWeaponBase>().VData;

                if (weaponData == null)
                    continue;

                weapon.Value!.Clip1 = weaponData.MaxClip1;
                weapon.Value!.ReserveAmmo[0] = weaponData.PrimaryReserveAmmoMax;
                Utilities.SetStateChanged(weapon.Value!.As<CCSWeaponBase>(), "CBasePlayerWeapon", "m_pReserveAmmo");
            }

            foreach (var player in players.Where(player => player != null && player.IsValid))
            {
                var currentHealth = player!.PlayerPawn.Value!.Health;

                player!.PlayerPawn.Value!.Health = Config.HealthRefill switch
                {
                    "all" => 100,
                    _ => currentHealth + int.Parse(Config.HealthRefill) >= 100 ? 100 : currentHealth + int.Parse(Config.HealthRefill)
                };

                Utilities.SetStateChanged(player!.PlayerPawn.Value!, "CBaseEntity", "m_iHealth");

                if (!Config.ArmorRefill)
                    continue;

                player!.PlayerPawn.Value!.ArmorValue = 100;
                Utilities.SetStateChanged(player!.PlayerPawn.Value!, "CCSPlayerPawn", "m_ArmorValue");
            }
        });

        return HookResult.Continue;
    }
}